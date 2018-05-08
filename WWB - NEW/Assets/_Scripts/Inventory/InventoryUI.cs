using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*InventoryUI
 * 
 * Updates the Inventory UI when an item is added or removed from the
 * player inventory.
 * 
 * 
 * Created By: Michelle Wolak 04/2018
 */

public class InventoryUI : MonoBehaviour {

	Inventory inventory;

	public Transform itemsParent;

	ItemSlot[] slots;

	/* Creates instance of Inventory if it is not already created and retrieves
	 * the list of slots from the inventory UI
	 */

	void Start () {
		inventory = Inventory.instance;	
		inventory.OnItemChangeCallBack += UpdateUI;

		slots = itemsParent.GetComponentsInChildren<ItemSlot> ();
	}
	/* Updates the Inventory slots with the items in the inventory.
	 * Will iterate through the whole list each time there is an update
	 */
	void UpdateUI () {

		for (int i = 0; i < slots.Length; i++) {
			if (i < inventory.inventory.Count) {
				slots [i].AddItem (inventory.inventory [i]);
			} else {
				slots [i].ClearSlot ();
			}
		}
	}
}
