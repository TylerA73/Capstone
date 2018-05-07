using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Consumable
 * 
 * Extends the Item class to make a Consumable type Item
 * An Asset Menu option is added for this item
 * 
 * 
 * Created By: Michelle Wolak 04/18
 */

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item {

	Inventory instance;

	GameObject player;

	public int health;
	public int strength;


	public override void Use ()
	{
		/* Any code in the original function will be run and then the code after it
		 * base.Use (); may be removed from this function
		 */ 
		base.Use ();
		player = GameObject.FindGameObjectWithTag ("Player");
		player.GetComponent<Stats> ().currentHealth += health;
		player.GetComponent<Stats> ().Strength += strength;
	}

}
