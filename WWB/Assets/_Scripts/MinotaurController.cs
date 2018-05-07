using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/**
 *  Author: Tyler Arseneault
 *  This is the initial Minotaur script
 *  I could not figure out why the Minotaur was not patrolling the Labyrinth
 *  I opted to rewrite a new script, called MinotaurController2, which is the one that is actually used
 *  While rewriting the script, I decided to create as close to a Compositie AI as I could with it
 */

public class MinotaurController : MonoBehaviour {

	private enum MinotaurState {
		PATROL, CHASE, ATTACK
	}

    const float WALK_SPEED = 2f;
    const float RUN_SPEED = 5f;

	public float speed;
    public Stats stats;
    public Transform[] patrol;
    private int destinationPoint;
    private Vector3 destination;

    private NavMeshAgent agent;
    public Transform target; //transform of the target gameobject
    private Transform player;
	private Transform transform; //transform of the AI
	private Rigidbody rbody; //rigidbody of the AI
 	private Animator animator;
	private MinotaurState state;

	/**
	 * Initializes important components for the AI
	 */
	void Start () {

		rbody = GetComponent<Rigidbody> ();
		transform = GetComponent<Transform> ();
        player = GameObject.FindGameObjectWithTag("Player").transform;
		animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
		state = MinotaurState.PATROL;
        destinationPoint = 0;
        destination = patrol[destinationPoint].position;
        agent.SetDestination(destination);
		stats = GetComponent<Stats>();

        //Stat initialization
        stats.maxHealth = 500;
        stats.Strength = 50;
        stats.Toughness = 20;
        stats.Critical = 0.1f;
        stats.Luck = 0.9f;
	}

	/**
	 * Updates the AI every frame
	 */
	void FixedUpdate () {

        switch (state)
        {
            case MinotaurState.PATROL:
                if (DistanceFromPlayer() <= 10f)
                    state = MinotaurState.CHASE;
                else
                    PatrolMap();
                break;
            case MinotaurState.CHASE:
                if (DistanceFromPlayer() < 1f)
                    state = MinotaurState.ATTACK;
                else if (DistanceFromPlayer() > 10f)
                    state = MinotaurState.PATROL;
                else
                ChasePlayer();
                break;
            case MinotaurState.ATTACK:
                if (DistanceFromPlayer() >= 1f)
                    state = MinotaurState.CHASE;
                Attack();
                break;
                
        }
        
    }

    //  Returns a float for the distance the enemy is away from the player
    float DistanceFromPlayer()
    {
        Vector3 p = this.player.position;
        return Vector3.Distance(p, transform.position);
    }

    //  Returns a float of the distance the enemy is from the current waypoint
    float DistanceFromWaypoint()
    {
        Vector3 waypoint = patrol[destinationPoint].position;
        float dist = Mathf.Abs(Vector3.Distance(waypoint, transform.position));
        return dist;
    }

    //  Handles the patrol actions of the AI
    void PatrolMap()
    {
        agent.speed = WALK_SPEED;
        animator.SetFloat("speed", agent.speed);
        animator.SetBool("isAttacking", false);
        if(DistanceFromWaypoint()<= 3f)
        {
            if(destinationPoint == patrol.Length - 1)
            {
                destinationPoint = 0;
                destination = patrol[destinationPoint].position;
            }
            else
            {
                destinationPoint++;
                destination = patrol[destinationPoint].position;
            }
        }

        agent.SetDestination(destination);
    }

    //  Handles the chasing player movements of the AI
    void ChasePlayer()
    {
        agent.speed = RUN_SPEED;
        animator.SetFloat("speed", agent.speed);
        animator.SetBool("isAttacking", false);
        agent.SetDestination(player.transform.position);
    }

    //  Handles the attack actions of the AI
    void Attack()
    {
        agent.speed = 0f;
        animator.SetFloat("speed", agent.speed);
        animator.SetBool("isAttacking", true);
    }

}
