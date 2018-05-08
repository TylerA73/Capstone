using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controller for player controller
// Built by: Ryan Bresnahan & Mark Reid
public class PlayerController2 : MonoBehaviour {

	public float walkSpeed = 5f;			// movement speed without speed modifiers
	public float sprintMult = 2f;			// multiplier applied to walkSpeed when sprinting
	public float jumpHeight = 10f;			// vertical magnitude of jump
	public float attackTime = 0;			// Time benchmark that determines if attacks are allowed
	public float attackCool = 0.5f;			// Time that must pass before another attack can be performed

	private float turnAmount;				// amount for character to turn
	private float forwardAmount;			// magnitude of forward motion
	private float groundCheckDist = 1.1f;	// distance from the ground
	private float origGroundCheckDist;		// initial groundCheckDist saved to variable

	private float gravMultiplier = 2.0f;	// multiplicative force applied to gravity for jumping
	private float movingTurnSpeed = 360;	// max speed of turning while moving
	private float stationaryTurnSpeed = 180;// max speed of turning while stationary
	private float animSpeedMultiplier = 1f;	// multiplicative speed adjustment for animations

	public Stats stats;						// reference to the scriptable stats class
	private Transform mainCam;				// main camera of the scene
	private Rigidbody rbody;				// rigidbody reference of the player object
	private Animator animator;				// reference to the asset animator
	private CapsuleCollider capsule;		// reference to the capsule collider
	private BoxCollider attackTrigger;		// Toggled area that determines if player's attack hits

	private Vector3 camForward;				// current direction that camera is facing
	private Vector3 movement;				// current direction of player movement
	private Vector3 groundNormal;			// normalized coordinates of ground

	public bool isAttacking;				// flag to indicate if an attack is in progress
	public bool isGrounded;					// flag to indicate whether player is touching ground
	public bool isSprinting;				// flag to indicate whether user has pressed sprint button
	public bool isJumping;					// flag to activate jumping condition

	public Slider healthBar;

	// Initialize components required for player functionality
	void Start () {
		rbody = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		capsule = GetComponent<CapsuleCollider>();
		attackTrigger = GetComponentInChildren<BoxCollider>();
		stats = GetComponent<Stats>();

		attackTrigger.enabled = false;
		origGroundCheckDist = groundCheckDist;
		rbody.constraints = RigidbodyConstraints.FreezeRotationX | 
								RigidbodyConstraints.FreezeRotationY | 
									RigidbodyConstraints.FreezeRotationZ;
		if (Camera.main != null)
			mainCam = Camera.main.transform;

		// Added by Tyler Arseneault
		// April 30,2018
		// Sets the cursor to invisible and locks it in place on start
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		healthBar.maxValue = stats.MaxHealth;
	}

	// Once per frame, check for user input and handle actions accordingly.
	// Additionally, a health check is performed to see if the player has been killed.
	// Attack timings assisted by Michelle Wolak
	private void Update()
	{
		healthBar.value = stats.currentHealth; // Added by Tyler Arseneault

		if(stats.currentHealth <= 0)
			PlayerDies();


		if(Input.GetButtonDown("Attack1") && !isAttacking)
		{
			isAttacking = true;
			attackTime = attackCool;
			attackTrigger.enabled = true;
			StartCoroutine("UseAttackOne");
		}

		if (Input.GetButtonDown("Attack2") && !isAttacking)
		{
			isAttacking = true;
			attackTime = attackCool;
			attackTrigger.enabled = true;
			StartCoroutine("UseAttackTwo");
		}

		if (isAttacking)
		{
			if (attackTime > 0)
				attackTime -= Time.deltaTime;
			else
			{
				isAttacking = false;
				attackTrigger.enabled = false;
			}
		}

		isSprinting = Input.GetKey(KeyCode.LeftShift);
		if (isGrounded)
			isJumping = Input.GetButtonDown("Jump");
	}

	// Once per physics frame detect user axis input and apply to 
	// player character.  Adjusts movement according to location of
	// camera. Set jumping state to false after each pass.
	private void FixedUpdate()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		if (mainCam != null)
		{
			camForward = Vector3.Scale(mainCam.forward, new Vector3(1, 0, 1)).normalized;
			movement = v * camForward + h * mainCam.right;
		}
		Move(movement, isJumping);
		isJumping = false;
	}

	// Function handles movement of character by taking a movement vector and applying
	// it along the plane to handle slopes.  Also applies a turn motion to the player.
	// Param = "move": current movement of the player character
	// Param = "jump": boolean value indicating whether the player is in a jumping state or not.
	private void Move(Vector3 move, bool jump)
	{
		if (move.magnitude > 1f)
			move.Normalize();
		move = transform.InverseTransformDirection(move);
		CheckGroundStatus();
		move = Vector3.ProjectOnPlane(move, groundNormal);
		turnAmount = Mathf.Atan2(move.x, move.z);
		forwardAmount = move.z;

		ApplyExtraTurnRotation();

		if (isGrounded)
			HandleGroundedMovement(jump);
		else
			HandleAirborneMovement();

		UpdateAnimator(move);
	}

	// Use raycast to adjust the state and groundNormal information of the player
	// depending on the return of the raycast.
	private void CheckGroundStatus()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo, groundCheckDist))
		{
			groundNormal = hitInfo.normal;
			isGrounded = true;
			animator.applyRootMotion = true;
		}
		else
		{
			isGrounded = false;
			groundNormal = Vector3.up;
			animator.applyRootMotion = false;
		}
	}

	// Smooth the player rotation using linear interpolation
	private void ApplyExtraTurnRotation()
	{
		float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
		transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
	}

	// Checks the status of the animator for permission to jump. Applies upward velocity
	// onto the rigidbody if animator is confirmed to be in Grounded state.
	// Param = "jump": boolean value indicating whether the player is in a jumping state or not.
	private void HandleGroundedMovement(bool jump)
	{
		if(jump && animator.GetBool("Grounded"))
		{
			rbody.velocity = new Vector3(rbody.velocity.x, jumpHeight, rbody.velocity.z);
			isGrounded = false;
			animator.applyRootMotion = false;
			groundCheckDist = 0.1f;
		}
	}

	// Apply gravity multiplier to airborne velocity and reset groundCheckDist
	private void HandleAirborneMovement()
	{
		Vector3 extraGravity = (Physics.gravity * gravMultiplier) - Physics.gravity;
		rbody.AddForce(extraGravity);
		groundCheckDist = rbody.velocity.y < 0 ? origGroundCheckDist : 0.01f;
	}

	// Apply current state and movement parameters to the animator.
	// Param = "move": current movement of the player character
	private void UpdateAnimator(Vector3 move)
	{
		animator.SetFloat("Speed", forwardAmount, 0.1f, Time.deltaTime);
		animator.SetBool("Grounded", isGrounded);
		if (!isGrounded)
			animator.SetBool("Jumping", true);
		if (isGrounded && move.magnitude > 0)
			animator.speed = animSpeedMultiplier;
		else
			animator.speed = 1;
	}

	//Apply movement to the player asset on animator call.
	private void OnAnimatorMove()
	{
		if (isGrounded && Time.deltaTime > 0)
		{
			if (isSprinting)
				rbody.velocity = movement * walkSpeed * sprintMult;
			else
				rbody.velocity = movement * walkSpeed;
		}
	}

	// Converts player input to compensate for current camera position and returns the
	// newly oriented Vector3 value.
	// Param = "v": Vertical magnitude of user input
	// Param = "h": Horizontal magnitude of user input
	// Built by: Ryan Bresnahan
	Vector3 AdjustMoveToCamera(float v, float h)
	{
		Camera camera = Camera.main;
		Vector3 forward = camera.transform.forward;
		Vector3 right = camera.transform.right;

		forward.y = 0f;
		right.y = 0f;
		forward.Normalize();
		right.Normalize();

		Vector3 desiredMoveDirection = forward * v + right * h;
		transform.Translate(desiredMoveDirection * walkSpeed * Time.deltaTime);
		return desiredMoveDirection;
	}

	// Begins coroutine for attack animation and toggles attack collision
	// Built by: Ryan Bresnahan
	IEnumerator UseAttackOne()
	{
		animator.SetBool("Slash Attack", true);
		yield return new WaitForSeconds(0.25f);
		animator.SetBool("Slash Attack", false);
	}

	// Begins coroutine for attack animation and toggles attack collision
	// Built by: Ryan Bresnahan
	IEnumerator UseAttackTwo()
	{
		animator.SetBool("Stab Attack", true);
		yield return new WaitForSeconds(0.25f);
		animator.SetBool("Stab Attack", false);
	}

	// Begins coroutine for jump animation
	// Built by: Ryan Bresnahan
	IEnumerator JumpAnim()
	{
		animator.SetBool("Jumping", true);
		yield return new WaitForSeconds(0.25f);
		animator.SetBool("Jumping", false);
	}

	// Applies basic stats to the player using the stats class's setters.
	// Built by: Ryan Bresnahan
	private void InitializeStats()
	{
		stats.maxHealth = 100;
		stats.currentHealth = stats.maxHealth;
		stats.Strength = 10;
		stats.Toughness = 10;
		stats.Critical = 5;
		stats.Luck = 5;
	}

	// Once defeat conditions are met for the player character the game object is destroyed.
	private void PlayerDies()
	{
		Destroy(gameObject);
	}

	// Get the stats object
	// Used in GameController
	public Stats GetStats ()
	{
		return stats;
	}

	// Get the stats object
	// Used in GameController
	public void SetStats (Stats newStats)
	{
		stats = newStats;
	}
}
