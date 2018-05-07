using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script applies a 'dumb-fire' motion to the predicted location of
// impact, removal after a maximum travel range, and handles interactions
// when another object is collided with.
// Built by: Ryan Bresnahan
public class ProjectileController : MonoBehaviour {

    private Rigidbody rb;

	public int damage = 15;				// Damage of projectile if hit
    public float speed = 10.0f;			// Speed of projectile default
	public float range = 40.0f;			// Maximum range of travel before deletion
    private Vector3 initial_loc;		// Spawn location of projectile

    // Initial setup of object, including rigidbody assignment and movement,
    // acquiring start location, and deflection calculations.
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        initial_loc = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        OutOfRange();
    }

    // Runs a calculation to determine whether the object has exceeded
    // 40 units from its origin location. If so, the object is destroyed.
    private void OutOfRange()
    {
        if (Vector3.Distance(transform.position, initial_loc) > range)
            Destroy(gameObject);
    }

	// On projectile's collision with player, inflict damage and destroy
	// the projectile.
	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			Stats player = other.GetComponent<Stats>();
			player.TakeDamage(damage);
			DestroyObject(gameObject);
		}
	}

	// If projectile collides with something other than the player,
	// destroy projectile and do nothing.
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag != "Player")
			DestroyObject(gameObject);
	}
}
