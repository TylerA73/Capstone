using UnityEngine;
using System.Collections;

/*
 * Controller for the camera rotation and distance
 * Adjustable clamp ranges and angles in editor.
 * Sourced from: http://wiki.unity3d.com/index.php?title=MouseOrbitImproved
 * Modified by: Ryan Bresnahan
 */

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{

    public Transform target;        // Game object that camera will follow
    public float distance = 5.0f;   // Range at which the camera follows by default
    public float homeDistance = 5.0f;    // Return value which camera will move to without collision
    public float xSpeed = 120.0f;   // Speed of horizontal camera rotation
    public float ySpeed = 120.0f;   // Speed of vertical camera rotation

    public float yMinLimit = 5f;    // Lowest angle camera can sit (in degrees)
    public float yMaxLimit = 80f;   // Highest angle camera cna sit (in degrees)

    public float distanceMin = .5f; // Closest distance camera can get to target
    public float distanceMax = 15f; // Furthest distance camera can get to target

    private Rigidbody rb;

    float x = 0.0f;
    float y = 0.0f;

    // Initial setup for camera angles and freezes rigidbody rotation.
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rb = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rb != null)
            rb.freezeRotation = true;
    }

    // Reads mouse movement and scroll wheel input to control camera.
    void LateUpdate()
    {
        x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        // Registers user input to set an offset value with the mousewheel. An initial value is determined and is 
        // used as a raycast target to check for collisions.
        homeDistance = Mathf.Clamp(homeDistance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
        Vector3 homeOffset = rotation * new Vector3(0.0f, 1.25f, -homeDistance) + target.position;

        // Reduces distance between camera and target if an object comes between. Assisted by Tyler Arsenault
        RaycastHit hit;
        if (Physics.Linecast(target.position, homeOffset, out hit) && hit.collider.tag != "Player")
            distance = hit.distance;
        else
            distance = homeDistance;

        // Calculates distance to hang back and sets position of camera
        Vector3 negDistance = new Vector3(0.0f, 1.25f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        // Sets the camera attributes to the final amounts at the end of each frame
        transform.rotation = rotation;
        transform.position = position;
    }
    
    // Error checking vertical camera limits
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}