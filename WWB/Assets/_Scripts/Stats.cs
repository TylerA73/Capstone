using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: Tyler Arseneault
 * Date: February 14, 2018
 * Description: Contains stats information for player and AI
 * It handles any damage to be taken, or to be dealt
 * Toughness will negate a specific amount of damage
 * Critical is a set value that can be added onto the strength when dealing damage
 * Critical will be added based on percentage between 0 and 100% of the total critical damage
 * If the percentage is 0%, then 0% of the critical will be added
 * If the percentage is 100%, then 100% of the critical stat will be added to strength
 * Luck is a float between 0 and 1
 * It will be added to the critical percentage upon damage, resulting in an increase in critical damage
 */
public class Stats : MonoBehaviour
{
    public int currentHealth;
	public int maxHealth;
	public int strength;
	public int toughness;
	public float critical;
	public float luck;

	// Value that represents the hit points of a creature. When the value reaches zero
	// it is typically considered defeated.
    public int CurrHealth {
        set { currentHealth = value; }
        get { return currentHealth; }
    }

	// Value that represents the hit points of a creature. When the value reaches zero
	// it is typically considered defeated.
	public int MaxHealth
	{
		set { maxHealth = value; }
		get { return maxHealth; }
	}

	// Value representing the base damage that a creature will inflict.
	public int Strength {
        set { strength = value; }
        get { return strength; }
    }

	// Value that negates a flat amount of damage that the creature receives.
    public int Toughness
    {
        set { toughness = value; }
        get { return toughness; }
    }

	// Value added onto strength when dealing damage 
    public float Critical {
        set { critical = value; }
        get { return critical; }
    }
	// Float between 0 and 1 that is added to critical percent upon damage to increase critical damage
    public float Luck
    {
        set { luck = value; }
        get { return luck; }
    }

    // Called when damage is inflicted to the creature.
	// Param = "dmg": Damage inflicted prior to reductions from the toughness stat.
    public void TakeDamage(int dmg){
		if (toughness >= dmg)	// Clause to prevent "healing hits" by Ryan Bresnahan
			dmg = toughness;
        if (currentHealth > 0)
			currentHealth -= (dmg - toughness);
        else
			currentHealth = 0;
    }

     // Handles the dealing of damage
    public int DealDamage(){
        
        float rnd = Random.value;

        // f(x) = x * (1 + fixed critical % value)
        if (rnd > luck)
            return Mathf.CeilToInt(strength * (1 + critical));

        return strength;
    }
}
