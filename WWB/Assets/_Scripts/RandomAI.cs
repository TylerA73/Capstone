using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class for an enemy AI
 *
 * Author: Daniel Brunelle
 * Attack collisions and death implemented by Tyler and Ryan
 */
public class RandomAI : MonoBehaviour
{
	enum State { Idle, Tracking, Roaming, Retreating }

	public GameObject target;   //Target player
	public float trackingDistance;  //Distance in which the AI is able to see that player
	public int speed;   //Speed that AI moves at
	public Swarm swarmManager;

	private Animator animator;
	private Stats stats;
	private float distance; // Distance between this ai and target player
	private State state;    //Current state of AI
	private Vector3 retreatDir;
	private BoxCollider attackBox; //	Author: Tyler Arseneault
	private float attackTime;				// Decrementing time since last attack
	private float attackCool = 0.5f;		// Minimum time between attacks

	// Use this for initialization
	void Start()
	{
		stats = GetComponent<Stats>();
		state = State.Idle;
		target = GameObject.Find("Player");
		attackBox = GetComponentInChildren<BoxCollider>();
		attackBox.enabled = false;
		animator = GetComponent<Animator>();
		attackTime = 0f;
		//StatInit();

	}

	// Update is called once per frame
	void Update()
	{
		attackTime = attackTime - Time.deltaTime;
		if(stats.CurrHealth <= 0f){
			Die();
		}
	}

	void FixedUpdate()
	{
		if (InSight())
		{
			swarmManager.AddToSwarm(transform.GetComponent<RandomAI>());
		}
		else
		{
			swarmManager.RemoveFromSwarm(transform.GetComponent<RandomAI>());
		}
		
		if (state == State.Retreating)
		{
			float moveSpeed = speed * Time.deltaTime;
			transform.position += retreatDir * moveSpeed;
			return;
		}

		bool inSight = InSight();
		if (inSight)
		{
			state = State.Tracking;
			Attack();
		}
		else if (!inSight)
		{
			state = State.Roaming;
			Roam();
		}
	}

	/*
	 *	Handles the damage dealt to the player by the enemy
	 *	Authors: Tyler and Ryan
	 */
	void OnTriggerStay(Collider other){
		int dmg = stats.DealDamage();
		if (other.tag == "Player" && attackTime <= 0f)
		{
			
			attackTime = attackCool;
			attackBox.enabled = false;
			Stats enemy = other.GetComponent<Stats>();
			enemy.TakeDamage(dmg);
		}
	}


	/*
	 * Initialize stats of the AI
	 *
	 * Author: Daniel Brunelle
	 */
	void StatInit()
	{
		stats.currentHealth = 100;
		stats.maxHealth = 100;
		stats.Strength = 10;
		stats.Toughness = 10;
		stats.Critical = 0;
		stats.Luck = 0;
	}

	/**
	* Returns true if target player is in direct line of sight of AI
	*
	* Author: Daniel Brunelle
	*/
	bool InSight()
	{
		distance = Vector3.Distance(transform.position, target.transform.position);
		Vector3 direction = target.transform.position - transform.position;

		if (distance < trackingDistance)
		{
			RaycastHit hit;
			Physics.Raycast(transform.position, direction, out hit);
			//if (hit.transform == target.transform || hit.transform.GetComponent<RandomAI>())
			if (hit.transform == target.transform)
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * Makes the ai roam pseudo randomly
	 * (turns if ai encounters an object in front of it)
	 *
	 * Author : Daniel Brunelle
	 */
	void Roam()
	{
		//Shoots front ray
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		Physics.Raycast(ray, out hit);

		//Shoots ray to the left
		Ray leftRay = new Ray(transform.position, -transform.right);
		RaycastHit leftHit;
		Physics.Raycast(leftRay, out leftHit);

		//Shoots ray to the right
		Ray rightRay = new Ray(transform.position, transform.right);
		RaycastHit rightHit;
		Physics.Raycast(rightRay, out rightHit);

		//checks if object is near it and determines which way it should turn
		//depending on if an object is closer on the left or right
		if (hit.distance < 4 || rightHit.distance < 1 || leftHit.distance < 1)
		{
			if (rightHit.distance > leftHit.distance)
			{
				transform.Rotate(0, 1, 0);
			}
			else
			{
				transform.Rotate(0, -1, 0);
			}
		}
		float moveSpeed = (speed / 2) * Time.deltaTime;
		transform.position = transform.position + transform.forward * moveSpeed;
	}

	/*
	 * Timed retreat function to cause AI's to run away for given amount of time
	 * param time - Time in seconds to retreat for
	 * 
	 * Author: Daniel Brunelle
	 */
	IEnumerator Retreating(int time)
	{
		yield return new WaitForSeconds(time);
		state = State.Roaming;
	}

	/*
	 * Method to set AI's to retreat mode
	 * 
	 * Author: Daniel Brunelle
	 */
	public void Retreat()
	{
		state = State.Retreating;
		retreatDir = -transform.forward;
		StartCoroutine(Retreating(5));
	}

	/**
	* Sets the AI to attack the target player
	*
	* Author: Daniel Brunelle
	*/
	void Attack()
	{
		if(distance < 2f && attackTime <= 0f){
			attackBox.enabled = true;
			animator.Play("attack01", -1);
		}
		float moveSpeed = speed * Time.deltaTime;
		transform.LookAt(target.transform);
		transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed);
	}

	/*
	 *	Handles the death of the AI
	 *	Author: Tyler Arseneault 
	 */
	void Die(){
		Destroy(this.gameObject);
	}
}


