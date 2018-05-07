using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Deprecated Class, now wrapped into the Stats class.
// EnemyHealth class is attached to creatures that are affected by
// player attacks and interactions.  
public class EnemyHealth : MonoBehaviour {

	public int startingHealth;
	public int currentHealth;
	public int strength;
	CapsuleCollider capsuleCollider;

	// Initialize reference to collider and set currentHealth
	void Start () {
		capsuleCollider = GetComponent<CapsuleCollider>();
		currentHealth = startingHealth;
	}
	
	// Accepts and integer value to deduct from the enemy's currentHealth.
	// If health is reduced to zero, enemy is defeated and is removed from scene.
	public void TakeDamage(int amount)
	{
		currentHealth -= amount;
		if (currentHealth <= 0)
			Death();
	}

	// Called when health is zero, removes enemy from scene.
	void Death()
	{
		Destroy(gameObject);
	}
}
