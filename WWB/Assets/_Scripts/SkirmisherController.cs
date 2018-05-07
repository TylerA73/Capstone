using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Skirmisher is a light melee unit that will approach and attack opponents when within 
// a certain distance.  Will not pursue outside of this aggression range.
// Built by: Ryan Bresnahan
public class SkirmisherController : MonoBehaviour {

	public Stats stats;                     // Reference to the scriptable stats class
	private Transform target;				// Reference to player character location
	private Animator anim;					// Animator that controls creature's animation clips
	private BoxCollider attackBox;			// Collider associated with creature's attacks

	public float speed;						// Adjustable movement speed of creature

	private float aggroDist = 20.0f;		// Range at which creature will engage target
	private float attackRange = 2.0f;		// Offset from target that creature will attack from
	private float playerDist;				// Distance from creature to player
	private float attackTime;				// Decrementing time since last attack
	private float attackCool = 0.5f;		// Minimum time between attacks

	// Retrieve reference to components needed for creature interaction
	void Start () {
		stats = GetComponent<Stats>();
		anim = GetComponent<Animator>();
		attackBox = GetComponentInChildren<BoxCollider>();
		target = GameObject.Find("Player").transform;
	}
	
	// Checks if player is within range and sight, and checks if creature has
	// been defeated.
	void Update () {
		playerDist = Vector3.Distance(transform.position, target.position);
		attackTime = attackTime - Time.deltaTime;

		if ((playerDist < aggroDist))
			Move();
		else
			anim.SetBool("Walking", false);

		if (stats.currentHealth <= 0)
			Dies();
	}

	// Function handles creature rotation, movement, and interaction with player
	public void Move()
	{
		// Look at player
		transform.LookAt(target.position);
		anim.SetBool("Walking", true);

		// Move toward player
		if (playerDist > attackRange)
			transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));

		// And give them what-for
		else if(attackTime <= 0)
			StartCoroutine(Attack());
	}

	// Initiates coroutine that runs the attack animation and deals damage to the target
	IEnumerator Attack()
	{
		attackBox.enabled = true;
		attackTime = attackCool;
		anim.Play("Attack", -1);
		yield return new WaitForSeconds(0.75f);
	}

	// Deal damage if the target is the player and disable collider until next attack.
	// Parameter 'other' is collider of another creature.
	void OnTriggerEnter(Collider other)
	{
		int dmg = stats.DealDamage();
		if (other.tag == "Player")
		{
			Stats enemy = other.GetComponent<Stats>();
			enemy.TakeDamage(dmg);
			attackBox.enabled = false;
		}
	}

	// Once defeat conditions are met the game object is destroyed.
	private void Dies()
	{
		Destroy(gameObject);
	}
}
