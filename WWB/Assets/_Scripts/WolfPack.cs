using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script handles creation of new Wolves pack of given size and location.
// Sets the target of the pack to the player's transformation. Happy hunting.
// Adapted from class scripts by: Ryan Bresnahan
public class WolfPack : MonoBehaviour {

	public Transform target;
	public Wolves pack;
	public GameObject prefab;

	// Create new Wolves pack at given location
	void Start () {
		pack = new Wolves("Timber", transform.position, 10f, 6, prefab);
	}

	// Reference player's location and provide Wolves with target transformation.
	private void Update()
	{
		target = GameObject.Find("Player").transform;
		pack.SetEnemy(target);
	}
}
