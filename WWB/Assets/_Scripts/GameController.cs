using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* GameController
 * 
 * Control the inventory menu, and transitioning between levels
 * 
 * Created by: Michelle Wolak 07/03/18
 * Modified by: Mark Reid 28/04/18
 */

public class GameController : MonoBehaviour
{

	public GameObject InventoryMenu; // The game object for the player's inventory

	private PlayerController2 player; // Controller for the player

	// Stop the gamecontroller object from unloading when its scene is unloaded
	void Awake () {
		DontDestroyOnLoad (transform.gameObject);
	}

	// Set up the first scene
	void Start() {
		//Load first game scene (Outdoor scene)
		if (SceneManager.GetActiveScene().name == "Start")
			SceneManager.LoadSceneAsync("Outdoor_Level");
	}

	// Control the state of the inventory menu
	// Adds an input for skipping to the second level
	void Update ()
	{
		/* Activate/Deactivate the Inventory menu and pause/unpause the game */
		if (Input.GetKeyDown(KeyCode.I)) { 
			InventoryMenu.SetActive (!InventoryMenu.activeSelf);
			if (InventoryMenu.activeSelf) { 
				Time.timeScale = 0.0f;

				// Sets the curser to be visible while viewing the inventory
				Cursor.visible = true; // Added by Tyler Arseneault April 30, 2018
				Cursor.lockState = CursorLockMode.None;	// Added by Tyler Arseneault April 30, 2018
			} else if (!InventoryMenu.activeSelf) {
				Time.timeScale = 1.0f;

				// Sets the cursor to not be visible when not viewing the inventory
				Cursor.visible = false;	// Added by Tyler Arseneault April 30, 2018
				Cursor.lockState = CursorLockMode.Locked; // Added by Tyler Arseneault April 30, 2018
			}
		}
		// A shortcut to the second level, used for testing/showing off
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			LoadLabyrinth ();
		}
	}

	// Load in the second level, and move the player's stats to the new player
	public void LoadLabyrinth () {
		Stats PlayerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2>().GetStats();
		SceneManager.LoadScene("_Labyrinth_Main");
		GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController2> ().SetStats (PlayerStats);
	}
}
