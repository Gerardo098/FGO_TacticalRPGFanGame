using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Movement_Root is the root abstract class for the player and NPC movement scripts.
/// This script handles most of the movement functions aside from specific functions exclusive to the two subclasses.
/// </summary>
public abstract class Movement_Root : MonoBehaviour
{
    // Variables
    public CharacterManager charMan; // Character manager reference

    GameObject[] nodes; // array of all nodes on the grid - aquired from the MC
    protected List<Node> selectableNodes = new List<Node>(); // List of nodes within the unit's movement range

    protected bool moving = false; // flag to check if our character is currently moving to a target node or not

    protected int maxMove; // Maximum distance
    protected float minDistance = 1f; // How close to the target do we have to be to know we've reached it

    protected Node currentNode; // Node the unit is currently occupying
    protected Node targetNode; // Node we're hoping to move to

    protected float jumpHeight = 2; // # of nodes that a unit can "climb" in their movement action
    protected Node actualTargetNode; // The target node for the unit's chosen movement

    // INIT() FUNCTIONS =====================================================================================

    /// <summary>
    /// Init() grabs the unit's Character_Root
    /// </summary>
    protected void Init()
    {
        // Grab the array of nodes from the MasterControl + the unit's max move
        nodes = charMan.master.GetNodes();
        maxMove = GetMovementValue();
    }

    /// <summary>
    /// GetMovementValue() finds and returns the unit's movement score
    /// </summary>
    /// <returns></returns>
    protected int GetMovementValue()
    {
        // Grab the unit's Movement value and return it
        charMan.character.parameterContainer.Get(charMan.MOV, out int movement);
        return movement;
    }

    // FINDING SELECTABLE NODES FUNCTIONS ====================================================================

    /// <summary>
    /// FindCurrentNode() checks for the node directly underneath the gameobject
    /// </summary>
    public void FindCurrentNode()
    {
        currentNode = FindTargetNode(gameObject);
        currentNode.current = true; // Set the node's current flag to true
    }

    /// <summary>
    /// ReturnCurrentNode() returns the node that the unit is occupying
    /// </summary>
    /// <returns> Occupied node's script </returns>
    public Node ReturnCurrentNode() { return currentNode; }

    /// <summary>
    /// FindTargetNode() utilizes a downward raycast hit to grab the node directly below the given target.
    /// If we hit something, return that node
    /// </summary>
    /// <param name="target"> Gameobject that we're checking the node of </param>
    /// <returns> Node reference </returns>
    protected Node FindTargetNode(GameObject target)
    {
        RaycastHit hit;
        Node node = null; // Temp storage for the found node

        // If the raycast hit a game object, grab the node component (if it has one)
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 10))
            { node = hit.collider.GetComponent<Node>(); }
        return node;
    }

    /// <summary>
    /// ComputeAdjacenecy() initializes the nodes by finding which nodes are adjacent to which,
    /// as well as which are within "jump height" of another.
    /// </summary>
    /// <param name="jump"> jump height; how high can a neighbouring node be before it's not "adjacent" </param>
    /// <param name="target"> Target node that we're looking to reach </param>
    public void ComputeAdjacency(float jump, Node target)
    {
        foreach (GameObject node in nodes)
        {
            // Grab the node script from each GameObject, and run its FindNeighbours() function
            Node n = node.GetComponent<Node>();
            n.FindNeighbours(jump, target);
        }
    }

    /// <summary>
    /// FindSelectable() goes through all nodes, starting from our current node, and looks outward at the other nodes.
    /// All nodes whose distance variable lies within our unit's max move are switched on to represent which are available.
    /// </summary>
    /*
     * FindSelectable() sets the flags for each node within the unit's move distance
     * to selectable (if walkable) 
     */
    protected void FindSelectable()
    {
        // Compute the adjacency of all nodes on the map
        ComputeAdjacency(jumpHeight, null);
        FindCurrentNode(); // Also, grab the unit's current node

        // A queue of nodes is used to process all nodes in order
        Queue<Node> process = new Queue<Node>();

        // Add the node we're currently standing on to the queue
        process.Enqueue(currentNode);
        currentNode.visited = true; // Turn on the visited flag

        // Continue this loop until we're out of nodes to process
        while (process.Count > 0)
        {
            Node node = process.Dequeue(); // Grab a node from the process queue
            selectableNodes.Add(node); // Add it to the list of available nodes
            node.selectable = true; // If it's in the selectableNodes list, it should be selectable

            if (node.distance < maxMove) // Check if the node is still within the unit's max movement
            {
                foreach (Node neighbour in node.adjacents) // Check the nodes' adjacents
                {
                    if (!neighbour.visited) // If we have yet to "visit" that node
                    {
                        // Update its info and throw it into the process queue
                        neighbour.parent = node;
                        neighbour.visited = true;
                        neighbour.distance = 1 + node.distance;
                        process.Enqueue(neighbour);
                    }
                }
            }
        }
    }
    // MOVEMENT FUNCTIONS =====================================================================================

    /// <summary>
    /// GrabTargetNode() takes a node, saves it as our target node, and lets the rest of the script know that
    /// we have a target, and should start to move by flipping the moving flag to true.
    /// </summary>
    /// <param name="target"> Target node we are looking to reach </param>
    protected void GrabTargetNode(Node target)
    {
        targetNode = target;
        moving = true; // We have a target, we can move now
    }

    /// <summary>
    /// Move() handles the movement of the unit's gameobject to the target node.
    /// </summary>
    protected void Move()
    {
        // Mark the target node as true and grab its position
        targetNode.target = true; 
        Vector3 targetPosition = targetNode.transform.position;

        // Set the destination of our navmeshagent to be the target node's position
        charMan.navMeshAgent.SetDestination(targetPosition);

        // Remove all selectable nodes from the field (so it looks better)
        RemoveSelectable();

        float difference = Vector3.Distance(transform.position, charMan.navMeshAgent.destination);

        // Once we're within minimum distance from the target node, tell the system we're no longer moving
        if (difference <= minDistance)
        {
            moving = false; // No longer moving
            AnchorPosition(); // Anchor the unit to its new position (hopefully avoid strange collision effects)
            charMan.character.ReduceActionCount(); // Movement uses up 1 action per turn
        }
    }

    /// <summary>
    /// AnchorPosition() simply sets the unit's velocity in the rigidbody to 0
    /// </summary>
    private void AnchorPosition()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    /// <summary>
    /// This version of AnchorPosition() is called upon by the turn manager;
    /// In an attempt to keep all units from moving after a collision from a navmeshagent movement, we run this script
    /// to keep the unit in place and leave their velocity at 0
    /// </summary>
    public void AnchorPosition_SOT()
    {
        // Find the unit's current node
        if (currentNode == null) { currentNode = FindTargetNode(gameObject); }

        // Grab the node's position and the unit's y position
        Vector3 NodePosition = currentNode.transform.position;
        float unitHeight = transform.position.y;

        // Find the position the unit should be standing at
        float NodeX = NodePosition.x;
        float NodeY = NodePosition.y + unitHeight;
        float NodeZ = NodePosition.z;
        Vector3 anchor = new Vector3(NodeX, NodeY, NodeZ);
        // Make the unit's position this position
        transform.position = anchor;

        // Finally, set the velocity to 0 to avoid any more moving
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    // CLEANUP FUNCTIONS ======================================================================================
    
    /// <summary>
    /// RemoveSelectable() is the opposite of the FindSelectable() function - we reset the values of all
    /// nodes back to their defaults.
    /// </summary>
    internal void RemoveSelectable()
    {
        // Reset the current node
        if (currentNode != null)
        {
            currentNode.current = false;
            currentNode = null;
        }
        // Reset all other nodes then clear our selectableNodes list
        foreach (Node node in selectableNodes) { node.Reset();}
        selectableNodes.Clear();
    }
}