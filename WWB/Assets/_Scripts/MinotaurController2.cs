using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 *	Author: Tyler Arseneault
 *	COMPOSITE AI FOR FINAL PROJECT
 *	Script for controlling the Minotaur movements and actions.
 */

public class MinotaurController2 : MonoBehaviour {

	public Transform []patrol;	// Patrol points
	public Transform retreat;
	public float WALK_SPEED;	// Walk speed of the AI
	public float RUN_SPEED;		// Run speed of the AI
	public Stats stats;
	public float attackTime = 0;			// Time benchmark that determines if attacks are allowed
	public float attackCool = 2f;			// Time that must pass before another attack can be performed

	public Transform player;
	private NavMeshAgent agent;	
	private Transform destination;
	private int pointIndex;
	private Transform transform;
	private Animator animator;
	private Grade grade; // Grade class for calculating patrol strength
	private BoxCollider attackBox;

	public bool isResting;
	public bool isHunting;
	public bool isAttacking;
	public bool isDead;

	// Use this for initialization
	void Start () {
		agent = gameObject.GetComponent<NavMeshAgent>();
		pointIndex = 0;
		destination = patrol[0];
		transform = GetComponent<Transform>();
		animator = GetComponent<Animator>();
		grade = new Grade("grade", 20f, 50f);
		isHunting = false;
		isResting = false;
		attackBox = GetComponentInChildren<BoxCollider>();

		//stats = new Stats();
		stats = GetComponent<Stats>();

		InvokeRepeating("Heal", 2.0f, 0.5f); // start the repeating Heal function
	}
	
	// Update is called once per frame
	void Update () {
		float action = Eval(Health());
		if(isDead){
			Debug.Log("Dead");
		}else if(action < 0f){
			Die();
		}else if(action == 1f && !isHunting && !isAttacking && !isResting){
			Patrol();
		}else if(action == 1f && isHunting && !isAttacking && !isResting){
			Hunt();
		}else if(action == 1f && isHunting && isAttacking && !isResting){
			if(attackTime <= 0){
				attackBox.enabled = true;
				StartCoroutine(Attack());
			}else{
				attackTime = attackTime - Time.deltaTime;
			}
		}else if( action < 1 && action > 0 && !isResting){
			Retreat();
		}else if(action == 0f && !isResting){
			FastRetreat();
		}else if(isResting){
			Rest();
		}
	}

	void OnTriggerStay(Collider other){
		int dmg = stats.DealDamage();
		if(other.tag == "Player"){
			Stats enemy = other.GetComponent<Stats>();
			enemy.TakeDamage(dmg);
			attackBox.enabled = false;
		}
	}

	/*
	 *	Description: Handle patrol movements of the AI
	 *	Param: N/A
	 *	Return: N/A
	 */
	void Patrol(){
		
		agent.speed = WALK_SPEED;
		SetAnimation(agent.speed);
		bool xEq = transform.position.x == destination.position.x; // are the x positions of the AI and patrol point equal?
		bool zEq = transform.position.z == destination.position.z; // are their z positions equal?
		
		// if they both are, then move onto the next patrol point
		if(xEq && zEq){
			if(pointIndex == patrol.Length-1){
				pointIndex = 0;
			} else{
				pointIndex++;
			}
			destination = patrol[pointIndex];
		}
		
		// set the next destination
		agent.SetDestination(destination.position);
		RaycastHit hit;
		if(Physics.SphereCast(transform.position, 2f, transform.forward, out hit)){
			if(hit.transform.tag == "Player"){
				isHunting = true;
				player = hit.transform;
			}
		}
	}

	/*
	 *	Description: Handle hunt movements of the AI. The AI tracks and follows the Player
	 *	Param: N/A
	 *	Return: N/A
	 */
	void Die(){
		//Destroy(this.gameObject);
		agent.speed = 0f;
		SetAnimation(agent.speed);
		animator.SetBool("isDying", true);
		isDead = true;
		Destroy(this.gameObject);
	}

	/*
	 *	Description: Handle hunt movements of the AI. The AI tracks and follows the Player
	 *	Param: N/A
	 *	Return: N/A
	 */
	void Hunt(){
		agent.speed = 5f;
		SetAnimation(agent.speed);
		agent.SetDestination(player.position);
		animator.SetBool("isAttacking", isAttacking);
		float distance = Mathf.Abs(transform.position.magnitude - player.position.magnitude);
		if(distance < 2f){
			isAttacking = true;
		}else if(distance > 4f){
			isHunting = false;
		}
	}

	/*
	 *	Description: Handle attack movements of the AI
	 *	Param: N/A
	 *	Return: N/A
	 */
	IEnumerator Attack(){
		attackTime = attackCool;
		agent.speed = 0f;
		SetAnimation(agent.speed);
		animator.SetBool("isAttacking", isAttacking);
		float distance = Mathf.Abs(transform.position.magnitude - player.position.magnitude);
		if(distance >= 2f){
			isAttacking = false;
		}
		yield return new WaitForSeconds(0.75f);
		isAttacking = false;
		animator.SetBool("isAttacking", false);
	}

	/*
	 *	Description: Handle retreat movements of the AI. The AI walks to retreat point to recover.
	 *	Param: N/A
	 *	Return: N/A
	 */
	void Retreat(){
		isHunting = false;
		isAttacking = false;
		animator.SetBool("isAttacking", isAttacking);
		agent.SetDestination(retreat.position);
		agent.speed = 2f;
		SetAnimation(agent.speed);
		float x = transform.position.x;
		float z = transform.position.z;

		if(x == retreat.position.x && z == retreat.position.z && stats.currentHealth < stats.MaxHealth){
			isResting = true;
		}
	}

	/*
	 *	Description: Handle fast retreat movements of the AI. The AI runs to the retreat point to recover
	 *	Param: N/A
	 *	Return: N/A
	 */
	void FastRetreat(){
		isHunting = false;
		isAttacking = false;
		animator.SetBool("isAttacking", isAttacking);
		agent.SetDestination(retreat.position);
		agent.speed = 5f;
		SetAnimation(agent.speed);

		float x = transform.position.x;
		float z = transform.position.z;

		if(x == retreat.position.x && z == retreat.position.z  && stats.currentHealth < stats.MaxHealth){
			isResting = true;
		}
	}

	/*
	 *	Description: Handle resting movements of the AI. The AI stands idle in the resting point
	 *	Param: N/A
	 *	Return: N/A
	 */
	void Rest(){
		agent.speed = 0;
		SetAnimation(agent.speed);
	}

	/*
	 *	Description: Evaluate some of the actions of the AI based on the AI health
	 *	Param: float health
	 *	Return: float grade
	 */
	float Eval(float health){
		return grade.Eval(health);
	}

	/*
	 *	Description: Set the animation for the AI
	 *	Param: float speed
	 *	Return: N/A
	 */
	void SetAnimation(float speed){
		animator.SetFloat("speed", agent.speed);
	}

	/*
	 *	Description: returns the health stat
	 *	Param: N/A
	 *	Return: int health
	 */
	int Health(){
		return stats.currentHealth;
	}

	/*
	 *	Description: Heals the AI by 5 points per second while resting
	 *	Param: N/A
	 *	Return: N/A
	 */
	void Heal(){
		if(isResting){
			stats.currentHealth += 5;
			if(stats.currentHealth >= stats.MaxHealth){
				stats.currentHealth = stats.MaxHealth;
				isResting = false;
			}
		}
	}
}
