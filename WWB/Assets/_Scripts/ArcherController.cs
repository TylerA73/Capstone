using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script dictates the behaviour of a ranged attacker.
// Handles target acquisition, leading of projectile attacks,
// and attack parameters.
// Built by: Ryan Bresnahan
public class ArcherController : MonoBehaviour {

    public float nextFire;              // Counter for fire delay
    public float fireDelay = 3.0f;      // Time between attacks
    public float fireRange = 20.0f;     // Range at which shots begin
    public float pSpeed = 10.0f;        // Speed of projectile
	private Stats stats;                // Reference to the scriptable stats class

	public GameObject target;           // Object to shoot towards
    public GameObject projectile;       // Prefab for instantiation

	public Transform shotSpawn;			// Location of projectile instantiation
    public Transform targetLocation;    // Location of target to shoot at
	private Transform model;			// Reference to the transform of the archer
    private Rigidbody rb;               // Body of the rigid variety


	// Initialize components required for interaction
	void Start() {
		rb = GetComponent<Rigidbody>();
		model = GetComponentInChildren<Transform>();
		stats = GetComponent<Stats>();
		target = GameObject.Find("Player");
    }

    // Update is called once per frame to verify health,
	// orient creature, and attack if in range.
    void Update() {
		if (stats.currentHealth <= 0)
			Dies();
		FireAtTarget();
		model.LookAt(target.transform);
	}

    
     // Performs a check on distance and time to verify whether a shot is allowed by the gameobject.
     // Calculates the trajectory needed to lead the target and instantiates from a prefab towards a 
     // target location.
    void FireAtTarget()
    {
        float distance = Vector3.Distance(target.transform.position, transform.position);
        if ((distance <= fireRange) && (Time.time > nextFire))
        {
            nextFire = Time.time + fireDelay;
            Vector3 ship_spd = target.GetComponent<Rigidbody>().velocity;
            Quaternion shot_orientation = GetDeflection(target.transform.position, ship_spd, shotSpawn.position, pSpeed);
            Instantiate(projectile, shotSpawn.position, shot_orientation);
        }
	}

     
    // Calculates the cross product angle on the y-axis rotation where a projectile and target will intercept,
    // uses sine law to determine the required angle of launch from origin point, converts the value to a
	// Vector3 by applying projectile speed to the normalized trajectory, and returns a Quaternion representing
    // the shot orientation needed to lead the target.
	private Quaternion GetDeflection(Vector3 targetLoc, Vector3 targetSpd, Vector3 launch_pt, float shot_spd)
    {
        float sin_theta = -Vector3.Cross(targetSpd.normalized, (targetLoc - launch_pt).normalized).y;
        float deflection = Mathf.Asin(sin_theta * (targetSpd.magnitude / shot_spd));
        deflection = Mathf.Rad2Deg * deflection;

        Vector3 projectile_forwards = (targetLoc - launch_pt).normalized * shot_spd;
        return Quaternion.Euler(0, deflection, 0) * Quaternion.FromToRotation(Vector3.forward, projectile_forwards);
    }

	// Applies basic stats to the player using the stats class's setters.
	private void InitializeStats()
	{
		stats.maxHealth = 25;
		stats.currentHealth = stats.maxHealth;
		stats.Strength = 5;
		stats.Toughness = 0;
		stats.Critical = 0;
		stats.Luck = 0;
	}

	// Once defeat conditions are met the game object is destroyed.
	private void Dies()
	{
		Destroy(gameObject);
	}
}
