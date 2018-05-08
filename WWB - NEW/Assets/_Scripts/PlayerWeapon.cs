using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script handles interaction of the player's attack collider
// Built by: Ryan Bresnahan
public class PlayerWeapon : MonoBehaviour {

	private Stats stats;			// Reference to the scriptable stats class
	private int damage;				// Damage inflicted to enemy creature
	private BoxCollider box;		// Reference to attack collider of player

	// Initialize player's stats for damage calculation and
	// reference to the attackable area.
	void Start () {
		stats = GetComponentInParent<Stats>();
		box = GetComponent<BoxCollider>();
	}

	// Trigger function that accepts an outside collider and applies damage
	// to the entity if it is tagged as an enemy. 
	private void OnTriggerStay(Collider other)
	{
		damage = stats.DealDamage();
		if(other.gameObject.tag == "Enemy")
		{
			Stats enemy = other.GetComponent<Stats>();
			enemy.TakeDamage(damage);
			box.enabled = false;
		}
	}
}
