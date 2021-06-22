using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement_Player is the script used by PC-controlled units, allowing the player to 
/// select the location to move to.
/// </summary>
public class Movement_Player : Movement_Root
{
    // Start is called before the first frame update
    void Start()
    { 
        // Init the Movement script - found in the Movement_Root script
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // Testing - making sure the pill is facing the right way when moving
        Debug.DrawRay(transform.position, transform.forward);

        // Check the moving bool flag, and activate the appropriate effect.
        if (!moving) // If we're not moving...
        {
            maxMove = GetMovementValue(); // Update the unit's movement value
            FindSelectable(); // Look for all avialable options among the nodes within our maximum movement
            CheckMouse(); // Check with out mouse+camera to see if our choice is legal
        }
        else // If we are moving...
        {
            Move(); // Call the move() function and get the unit to move
        }
    }

    /// <summary>
    /// OnDisable() activates the RemoveSelectable() script when the script is turned off
    /// </summary>
    void OnDisable() { RemoveSelectable(); }

    /// <summary>
    /// CheckMouse() lets the user select a node to move to using a raycast from the camera.
    /// </summary>
    public void CheckMouse()
    {
        // Check for mouse click
        if (Input.GetMouseButtonUp(0))
        {
            // Ray variables
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the raycast hits an object...
            if (Physics.Raycast(ray, out hit))
            {
                //Check that we've hit an object tagged as "Node"
                // and whether it's selectable - within movement range
                if (hit.collider.CompareTag("Node"))
                {
                    Node node = hit.collider.GetComponent<Node>();
                    // If the target is legal, save the target node
                    if (node.selectable) { GrabTargetNode(node); }
                }
            }
        }
    }
}

/*
 * Use a Sphere to detect legal moves
 * Try to use A*
 * Health bar set active then turn off
 * Documentation is CRITICAL
 * 
 * 1). What worked well

2). What was tried and didn't work?

3). Any ideas you have for extending the project


Dictionaries also help
23rd is the MAX
 */