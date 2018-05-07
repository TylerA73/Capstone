using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ItemSlot
 * 
 * Updates the inventory slot with the item icon and a reference to the item
 * 
 * Created By: Michelle Wolak 04/2018
 */

public class ItemSlot : MonoBehaviour {

	public Image icon;

	Item item;

	// Updates the icon sprite to the current item on Add
	public void AddItem (Item newItem) {
		
		item = newItem;

		icon.sprite = item.itemImg;
		icon.enabled = true;

	}

	// Clears the icon sprite on item Removal
	public void ClearSlot() {

		item = null;

		icon.sprite = null;
		icon.enabled = false;
	}

	/* This function is linked to OnClick() of the item slot as the slots are buttons
	 * Also removes the item from the inventory on use
	 */
	public void UseItem () {

		if (item != null) {
			item.Use ();
			Inventory.instance.Remove (item);
		}
	}


}

