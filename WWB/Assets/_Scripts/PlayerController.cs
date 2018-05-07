using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Controller for the player character
 * Allows walking, sprinting, and jumping
 * Built by: Mark Reid
 */

public class PlayerController : MonoBehaviour {

    public float MovementSpeed = 5f;
	public float SprintSpeed = 2f; // Speed multiplier when sprinting
	public float JumpHeight = 5f; // Height of the jump
	public float pubspeed;

	private AnimatorStateInfo state; // Keep track of the character's state
	private Transform tform; // For getting direction
	private Rigidbody rbody; // For controlling movement
	private Vector3 groundNormal; // Normal of the ground on which the character stands
	private Vector3 speed; // Speed at which the player is moving
	private Animator animator;

	// Set up initial state, grab components
	void Start () {
		tform = GetComponent<Transform> ();
		rbody = GetComponent<Rigidbody> ();
		animator = GetComponent<Animator> ();
	}

	// Handle input for movement
	void FixedUpdate () {
		state = animator.GetCurrentAnimatorStateInfo (0);
		CheckGroundState ();
		Walk (Input.GetAxis ("Vertical"), Input.GetAxis ("Horizontal"), Input.GetButton ("Sprint"));
		animator.SetFloat ("Speed", speed.z * 0.5f);
		animator.SetBool("Backwards", speed.z < 0);
	}

    /*
	 * Move the player along the z and x axis
	 * z: magnitude of movement along z axis
	 * x: magnitude of movement along x axis
	 */
    void Walk(float z, float x, bool sprinting)
    {
        float ySpeed = rbody.velocity.y;
        //Vector3 targetSpeed = (z * tform.forward * 5f) + (x * tform.right * 5f);
        Vector3 targetSpeed = AdjustMoveToCamera(z, x);

		if (sprinting && z > 0)
        {
            targetSpeed *= SprintSpeed;
        }
        speed = Vector3.ProjectOnPlane(Vector3.Lerp(speed, targetSpeed, 0.1f), groundNormal);
        speed.y = ySpeed;
        rbody.velocity = speed;
    }

    /*
     * Converts player input to compensate for current camera position and returns the
     * newly oriented Vector3 value.
     * Built by: Ryan Bresnahan
     */
    Vector3 AdjustMoveToCamera(float z, float x)
    {
        Camera camera = Camera.main;
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = forward * z + right * x;
        transform.Translate(desiredMoveDirection * MovementSpeed * Time.deltaTime);
        return desiredMoveDirection;
    }

	/*
	 * Make the character jump
	 * Called from the animator
	 */
	void Jump () {
		if (animator.GetBool ("Grounded")) {
			rbody.velocity = new Vector3 (rbody.velocity.x, JumpHeight, rbody.velocity.z);
			animator.SetBool ("Jumping", false);
		}
	}

	/*
	 * Check if the character is standing on ground
	 * if they are, stop them from jumping and set the groundNormal
	 * otherwise make sure the state is isJumping
	 */
	void CheckGroundState () {
		RaycastHit hitInfo;

		if (Physics.Raycast (transform.position + (Vector3.up * 1f), Vector3.down, out hitInfo, 1.1f)) {
			groundNormal = hitInfo.normal;
		} else {
			groundNormal = Vector3.up;
		}
	}
}
