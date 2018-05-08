using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ItemObj
 * 
 * This script is to be attached to the item gameObject and contains
 * a reference to the specific item it is.
 * 
 * Created By: Michelle Wolak 04/2018
 */

public class ItemObj : Interactable {

	public Item item;


	public override void Interact () {
		PickUpItem ();
	}

	/* If the player is close enough and the R key is pressed the item will be
	 * picked up and added to the player inventory
	 */
	void Update () {
		if (Input.GetKey (KeyCode.R) && pickable) {
			Interact ();
		}
	}

	// Adds the item to the player's inventory
	void PickUpItem () {
		bool added = Inventory.instance.Add(item);
		if (added)
			Destroy (gameObject);
	}

}