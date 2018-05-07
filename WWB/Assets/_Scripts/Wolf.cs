using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pack unit that stays close to other units of the pack.  Adjusts
// its location depending on distance from other pack units and 
// location of target.
// Adapted from class scripts by: Ryan Bresnahan
public class Wolf : MonoBehaviour {
	private const int CYCLES = 5;								// Frames before decision process is executed.
	public float MAX_DIRECTION_CHANGE = 90f * Mathf.Deg2Rad;	// Maximum turn rate of a given frame.

	public float OWN_AVOIDANCE_WEIGHT = 0.05f;					// How strongly the unit prefers to keep its distance.
	public float OWN_AVOIDANCE_RADIUS = 5f;						// Radius that unit prefers to keep from pack units.

	public float OWN_ALIGNMENT_WEIGHT = 0.05f;					// How strongly the unit will seek to match pack speed.
	public float OWN_ALIGNMENT_RADIUS = 10f;					// Radius that unit is inclined to match speed to.

	public float OWN_COHESION_WEIGHT = 0.5f;					// How strongly the unit will seek to stay within pack formation.
	public float OWN_COHESION_RADIUS = 10f;						// Radius that unit seeks to stay within from pack formation.

	public float ENEMY_COHESION_WEIGHT = 0.75f;					// How strongly the unit will be drawn toward an enemy target.
	public float ENEMY_COHESION_RADIUS = 20f;					// Radius that unit will begin to engage the target from.

	public float baseSpeed = 2f;								// Base running speed, adjustable
	public float maxRunSpeed = 3.5f;							// Maximum speed a running speed, adjustable

	public List<GameObject> members;							// List of pack members
	public Transform enemy;										// Location of enemy (typically the player)

	private int cycles = CYCLES;								// Initialization of cycles variable.

	public Vector3 velocity, newVelocity;						// Direction and target direction to move the unit.

	// Handles behaviour of swarm members every so many "cycles" in order to minimize cost.
	// Each frame decrements wait time until next decision.
	void Update()
	{
		cycles--;
		if (cycles <= 0)
		{
			cycles = CYCLES;

			// Initialize unit preference variables
			Vector3 _separation, _alignment, _cohesion, _enemyCoh;
			Vector3 separationSum = Vector3.zero;
			Vector3 alignmentSum = Vector3.zero;
			Vector3 cohesionSum = Vector3.zero;

			int separationCount = 0;
			int alignmentCount = 0;
			int cohesionCount = 0;

			//Iterate through other pack members to get cumulative decision values.
			foreach (GameObject member in members) {
				if (member == null)
					continue;

				// Distance from this unit to selected unit.
				float distance = Vector3.Distance(transform.position, member.transform.position);

				// Ignore self
				if (distance > 0) {

					// Check if too close to formation
					if (distance < OWN_AVOIDANCE_RADIUS) {
						Vector3 direction = transform.position - member.transform.position;
						separationSum += direction;
						separationCount++;
					}

					// Check if not keeping up to formation
					if (distance < OWN_ALIGNMENT_RADIUS) {
						alignmentSum += member.GetComponent<Wolf>().velocity;
						alignmentCount++;
					}

					// Check if too far from formation
					if (distance > OWN_COHESION_RADIUS) {
						cohesionSum += member.transform.position;
						cohesionCount++;
					}
				}
			}

			// Find average of preference values from the number of impactful units.
			_separation = separationCount > 0 ? separationSum / separationCount : separationSum;
			_separation.Normalize();

			_alignment = alignmentCount > 0 ? Limit(alignmentSum / alignmentCount, MAX_DIRECTION_CHANGE) : alignmentSum;
			_alignment.Normalize();

			_cohesion = cohesionCount > 0 ? Steer(cohesionSum / cohesionCount, false) : cohesionSum;
			_cohesion -= transform.position;
			_cohesion.Normalize();

			// Determine enemy cohesion preference value.
			Vector3 enemyCohSum = Vector3.zero;
			int enemyCohCount = 0;
			float eDistance = Vector3.Distance(transform.position, enemy.position);

			if (eDistance < ENEMY_COHESION_RADIUS)
			{
				enemyCohSum += enemy.position;
				enemyCohCount++;
			}

			_enemyCoh = enemyCohCount > 0 ? Steer(enemyCohSum / enemyCohCount, false) : enemyCohSum;
			_enemyCoh -= transform.position;
			_enemyCoh.Normalize();

			// Get current velocity of unit and apply preference metrics to determine a new velocity .
			Vector3 oldVelocity = GetComponent<Wolf>().velocity;
			newVelocity = oldVelocity + _separation * OWN_AVOIDANCE_WEIGHT + _alignment * OWN_ALIGNMENT_WEIGHT + _cohesion * OWN_COHESION_WEIGHT
				 + _enemyCoh * ENEMY_COHESION_WEIGHT;

			newVelocity.y = 0f;
			newVelocity = Vector3.RotateTowards(oldVelocity, Limit(newVelocity, maxRunSpeed), MAX_DIRECTION_CHANGE * Time.deltaTime, maxRunSpeed * Time.deltaTime);
		}
	}

	// At the end of each frame set unit's velocity to calculated preferred velocity.
	// Also apply rotation toward new goal.
	void LateUpdate()
	{
		velocity = newVelocity;
		transform.position += newVelocity * Time.deltaTime;
		if (velocity != Vector3.zero)
			transform.rotation = Quaternion.LookRotation(velocity);
	}


	protected virtual Vector3 Steer(Vector3 target, bool slowDown)
	{
		Vector3 steer = Vector3.zero;
		Vector3 targetDirection = target - transform.position;
		float targetDistance = targetDirection.magnitude;

		transform.LookAt(target);

		if (targetDistance > 0)
		{
			targetDirection.Normalize();

			if (slowDown && targetDistance < 100f * baseSpeed)
			{
				targetDirection *= (maxRunSpeed * targetDistance / (100f * baseSpeed));
				targetDirection *= baseSpeed;
			}
			else
			{
				targetDirection *= maxRunSpeed;
			}

			steer = targetDirection - GetComponent<Rigidbody>().velocity;
			steer = Limit(steer, MAX_DIRECTION_CHANGE * Time.deltaTime);
		}

		return steer;
	}

	protected Vector3 Limit(Vector3 v, float max)
	{
		return (v.magnitude > max) ? v.normalized * max : v;
	}
}
