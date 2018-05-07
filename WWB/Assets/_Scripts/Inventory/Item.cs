using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Item
 * 
 * Base Scriptable Object class to be used for base items.
 * Creates a new item data file with the base item attributes.
 * 
 * Created By: Michelle Wolak 04/2018 
 */

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

	public string itemName;
	public string description;
	public int grade;
	public int itemID;
	public Sprite itemImg;
	public ItemType type;
	public bool collectable;

	public enum ItemType {CONSUMABLE, WEAPON, KEYITEM}


	/*
	 * Use() allows items to be used in different ways. But all items may be used.
	 */
	public virtual void Use () {
		// Use Item. Code to be added/written for each type of item (Consumbable, Weapon, KeyItem)
	}

 
}