using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Inventory
 * 
 * Allows for only a single instance of the inventory for the player.
 * Uses a delegate for update of the InventoryUI when an item is added or removed
 * from the inventory.
 * 
 * 
 * Created By: Michelle wolak 04/2018
 */

public class Inventory : MonoBehaviour {



	public List<Item> inventory = new List<Item> ();
	public int maxSpace = 20;

	public delegate void OnItemChange ();
	public OnItemChange OnItemChangeCallBack;

	#region Inventory Singleton
	public static Inventory instance;

	public void Awake () {

		if (instance != null) {
			return;
		}
		instance = this;
	}

	#endregion

	// Adds item to the inventory and invokes UI update. Inventory has a max space
	public bool Add (Item item) {
		if (inventory.Count >= maxSpace) { 
			return false;
		}
		inventory.Add (item);

		if (OnItemChangeCallBack != null)
			OnItemChangeCallBack.Invoke ();
		
		return true;
	}

	// Removes Item from inventory and invokes UI update
	public void Remove (Item item) {

		inventory.Remove (item);

		if (OnItemChangeCallBack != null)
			OnItemChangeCallBack.Invoke ();
	}
}
