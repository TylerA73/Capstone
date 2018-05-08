using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiCamera : MonoBehaviour {

	//private variables
	private float xRot;
	private float yRot;
	private float yRotMin = 1;
	private float yRotMax = 80;
	private Vector3 rotSmoothenV;
	private Vector3 currentRot;

	//public variables
	public float rotationSmoothen = 1.1f;
	public Transform target;
	public float cursorSensitivity = 10;
	public float targOffset = 4;

	void Update(){
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	// LateUpdate to be applied after every other Update method after a frame
	void LateUpdate () {
		xRot += Input.GetAxis ("Mouse X");
		yRot -= Input.GetAxis ("Mouse Y"); // minus because we want non inverted rotation

		//currentRot = Vector3.SmoothDamp (currentRot, new Vector3 (yRot, xRot), ref rotSmoothenV, rotationSmoothen);

		//apply clamp to the yRot so that you can't rotate over the player, can only rotate so far.
		yRot = Mathf.Clamp(yRot, yRotMin, yRotMax);
		
		transform.eulerAngles = new Vector3 (yRot, xRot); //apply mouselook to transform of the camera this script is attached to

		transform.position = target.position - transform.forward * targOffset; //place camera in position to see player and apply offset
	}
}
