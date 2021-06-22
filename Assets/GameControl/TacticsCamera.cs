using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// TacticsCamera is a class that controls the camera via the mouse, allowing the player to change the 
/// camera position whenever they want.
/// This code is based on the code written by https://www.youtube.com/watch?v=3B3UbYdHbmk
/// </summary>

public class TacticsCamera : MonoBehaviour
{
    Transform target; // Target we're focused on
    Transform rotationTarget; // Camera nest we're rotating
    Vector3 lastPostion;
    // Zoomin'
    private float zoomDistance; // How far we're zoomed out
    private float scrollSensitivity = 3; // How fast we zoom in/our
    private float minScroll = 3f; // Minimum zoom
    private float maxScroll = 10f; // Maximum zoom
    // Rotatin'
    private float rotateSensitivity = 0.5f; // How fast the camera rotation is

    /// <summary>
    /// Init(); we find the rotationTarget
    /// </summary>
    public void Init()
    {
        rotationTarget = transform.parent;
        GrabTarget(); // Find the actual target
    }

    /// <summary>
    /// FocusTarget() sets a new parent (unit to focus on) for the rotation target
    /// </summary> 
    /// <param name="obj"> object to move onto </param>
    public void FocusTarget(Transform obj)
    {
        rotationTarget.SetParent(obj); // Move the nest onto the parent
        GrabTarget(); // Find the actual target
    }

    /// <summary>
    /// GrabTarget() grabs the unit themselves and sets the current zoom distance to 
    /// the distance between them and the camera.
    /// </summary>
    private void GrabTarget()
    {
        target = rotationTarget.transform.parent; // Grab the unit here
        // Set the zoom distance
        zoomDistance = Vector3.Distance(target.position, transform.position);
    }

    /// <summary>
    /// Update(); the camera follows the target unit
    /// </summary>
    private void Update()
    {
        // Focus on the target
        transform.LookAt(target);
        // Find the position of the mouse cursor when either the right mouse button (1) or the mouse wheel (2)
        // are pressed down.
        if (Input.GetMouseButtonDown(1)) { lastPostion = Input.mousePosition; }

        Zoom();
        Orbit();
    }

    /// <summary>
    /// Zoom() allows the player to move the camera towards or away from the target.
    /// Using a clamp, we can prevent the player from clipping into the unit or zooming too far to
    /// not being able to see anything.
    /// </summary>
    public void Zoom()
    {
        // We control zoom with the scrollwheel
        float num = Input.GetAxis("Mouse ScrollWheel");
        zoomDistance -= num * scrollSensitivity;
        // Use Clamp to prevent the zoom from getting too close to a character or too far to see anything
        zoomDistance = Mathf.Clamp(zoomDistance, minScroll, maxScroll);
        Vector3 pos = target.position; // position of the target we are after
        pos -= transform.forward * zoomDistance; // Move towards pos across the negative forward axis
        // Set the camera's position accordingly
        transform.position = Vector3.Lerp(transform.position, pos, scrollSensitivity); 
    }

    /// <summary>
    /// Orbit() allows the player to move the camera around the unit while holding the right mouse button.
    /// You can orbit in all directions so long as the cursor remains in the screen.
    /// </summary>
    public void Orbit()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastPostion;
            float angleY = -delta.y * rotateSensitivity;
            float angleX = delta.x * rotateSensitivity;

            // X Axis angle creation
            Vector3 angles = rotationTarget.transform.eulerAngles;
            angles.x += angleY;
            angles.x = Mathf.Clamp(angles.x, 10f, 80f);

            rotationTarget.transform.eulerAngles = angles;

            rotationTarget.transform.RotateAround(target.position, Vector3.up, angleX);

            // Set the last position here for later use
            lastPostion = Input.mousePosition;
        }
    }
}
