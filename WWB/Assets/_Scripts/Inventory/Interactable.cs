using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Interactable
 * 
 * The class is to set an item gameObject as interactable by the player.
 * The Game object must have a Trigger attached.
 * 
 * Created By: Michelle Wolak 04/2018
 */

public class Interactable : MonoBehaviour {

	public float radius = 3;
	public bool pickable = false;

	// Method meant to be overwritten
	public virtual void Interact () {}
		

	void OnDrawGizmosSelected () {

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (transform.position, radius);

	}

	// Tags the item as pickable for the player
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			pickable = true;
		}
	}

	/* Tags the item as not pickable so the item will not be picked up
	 * if the player is not near it
	 */
	void OnTriggerExit (Collider other) {
		if (other.tag == "Player") {
			pickable = false;
		}
	}

}
