using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    public GameObject endPoint;
    public GameObject wall;
    private float moveSpeed;
	// Use this for initialization
	void Start ()
    {
        moveSpeed = 5.0f;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            wall.transform.position =  Vector3.Lerp(wall.transform.position, endPoint.transform.position, moveSpeed);
            Destroy(this.gameObject);
        }
    }
}
