using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Instantiates and sets up metrics for a wolf pack.
// Adapted from class scripts by: Ryan Bresnahan
public class Wolves : MonoBehaviour {

	public int wolfCount;
	public float spawnRadius;
	public string name;

	public List<GameObject> wolves;

	// Accepts a prefab and parameters to instantiate several units as a pack
	// Parameter 'nm' is a name that will be prepended to instantiation.
	// Parameter 'center' is location for creation of pack.
	// Parameters 'radius' is the minimum distance between the 'count' number of units.
	// 'Prefab' is the assembled gameobject with animator, model, and scripts.
	public Wolves(string nm, Vector3 center, float radius, int count, GameObject prefab)
	{
		if (prefab == null)
		{
			Debug.Log("Prefab is missing");
			return;
		}

		wolfCount = count;
		spawnRadius = radius;
		name = nm;

		// Instantiate the prefabs
		GameObject wolfTemp;
		Wolf wScript = null;
		wolves = new List<GameObject>();

		for (int i = 0; i < wolfCount; i++)
		{
			wolfTemp = (GameObject)GameObject.Instantiate(prefab);
			wolfTemp.name = name + " Doggo " + i;
			wScript = wolfTemp.GetComponent<Wolf>();
			wScript.members = wolves;

			// Spawn at incremental locations
			wolfTemp.transform.position = new Vector3(center.x + 3f * (int)(i / 4), 0.0f, center.z + 2f * (i % 4));
			wolves.Add(wolfTemp);
			wScript.velocity = Vector3.zero;
			wScript.newVelocity = wScript.velocity;
			wolfTemp.GetComponent<Rigidbody>().velocity = wScript.velocity;
		}
	}

	// Accepts a tranformation location associated with the targeted enemy.
	// Iterates through the list of swarm members and sets each member's target
	// to that transform.
	public void SetEnemy(Transform enemy)
	{
		foreach (GameObject wolf in wolves)
		{
			Wolf wScript = wolf.GetComponent<Wolf>();
			wScript.enemy = enemy;
		}
	}
}
