using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Change the scene to the labyrinth when the player touches this object
 * 
 * Created by Mark Reid on 29/04/18
 */
public class EnterLabyrinth : MonoBehaviour {
	private GameController controller;

	void Start () {
		controller = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		if (controller == null)
			Debug.Log ("Dang");
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Player") {
			controller.LoadLabyrinth ();
		}
	}
}
