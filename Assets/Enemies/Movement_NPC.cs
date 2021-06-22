using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement_NPC is the Movement_Root subclass used by units not controlled by the character.
/// At the moment, all NPC units share the same movement script - that is, move towards the closest enemy unit if
/// there is no enemy within range of an attack/ability.
/// Whether we move or not is handled by an NPC's personal character class.s
/// </summary>
public class Movement_NPC : Movement_Root
{
    GameObject target;

    /// <summary>
    /// Call the Movement_Root Init() function at the start
    /// </summary>
    void Start()
    {
        Init();
    }

    /// <summary>
    /// In this script's update, we handle movement similarly to the Movement_Player subclass.
    /// However, instead of relying on a raycast to find our target, we find the closest enemy unit
    /// and we calculate the shortest path towards them.
    /// </summary>
    void Update()
    {
        // Testing - making sure the pill is facing the right way
        Debug.DrawRay(transform.position, transform.forward);

        // If statement separating whether the unit is moving or not
        if (!moving)
        {
            FindNearest(); // Find the enemy closest to the unit
            maxMove = GetMovementValue(); // Get the unit's movement
            CalculatePath(); // Find the path using A*
            FindSelectable(); // Display selectable nodes (just for looks, AI doesn't need them)
        }
        else
        {
            Move(); // Move the unit
            this.enabled = false; // Turn the movement script off
        }
    }

    /// <summary>
    /// CalculatePath() checks if the target unit is not null.
    /// Then, it finds the node of our target and looks for a path.
    /// </summary>
    private void CalculatePath()
    {
        // If we have a target to move towards...
        if (target != null)
        {
            // Find the target's node and find the most efficient path to it
            Node targetNode = FindTargetNode(target);
            FindPath(targetNode);
        }
    }

    /// <summary>
    /// FindNearest() grabs all game objects with the "Player" tag and compares the distance between
    /// this unit and each game object.
    /// The gameobject with the lowest distance is selected as the target.
    /// </summary>
    private void FindNearest()
    {
        // Grab all "player"-tagged gameobjects
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        // nearestTarget holds out current closest target and distance will hold its distance
        // from this unit
        GameObject nearestTarget = null;
        float distance = Mathf.Infinity;

        foreach (GameObject possibleTarget in targets)
        {
            // Find the distance between this unit and possible target
            float d = Vector3.Distance(transform.position, possibleTarget.transform.position);

            // If this found distance is smaller than our previously saved distance,
            // replace the prev. saved target with this closer target, as well as their distance
            if (d < distance)
            {
                nearestTarget = possibleTarget;
                distance = d;
            }
        }
        
        // The nearestTarget is then saved as our closest target
        target = nearestTarget;
    }

    /// <summary>
    /// FindPath() uses A* pathfinding to dictate the path this unit should take to reach our
    /// currently selected target.
    /// </summary>
    /// <param name="target"> Position of the curent target </param>
    public void FindPath(Node target)
    {
        // Calculate the adjacency of all nodes, in respects to our target
        ComputeAdjacency(jumpHeight, target);
        FindCurrentNode(); // Grab our current node

        // Generate the lists for un-processes nodes (open) and processed nodes (closed)
        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        // Calculate the current node's f, g, and h costs
        // currentNode's g value should be 0 at this moment
        open.Add(currentNode);
        currentNode.h = CalculateHCost(currentNode, target);
        currentNode.f = currentNode.h;

        while (open.Count > 0) // Go through each node in the open list
        {
            // Get the lowest F cost among the nodes
            // cur for current
            Node cur = FindLowestF(open);
            closed.Add(cur);

            // Check if we've reached the target node
            if (cur == target)
            {
                actualTargetNode = FindEndNode(cur);
                GrabTargetNode(actualTargetNode);
                return;
            }

            // If we're not at the target node yet, let's check the adjacent nodes of our current node
            foreach (Node node in cur.adjacents)
            {
                // If the tile is in the closed list - do nothing;  we've already processed that node
                if (closed.Contains(node)) { }
                // If the adjacent node is in the open list
                else if (open.Contains(node))
                {
                    // Calculate a temp g from our current node + distance between current and target adjacent
                    float tempG = cur.g + Vector3.Distance(node.transform.position, cur.transform.position);

                    // If tmpG < adjacent node's g value
                    if (tempG < node.g)
                    {
                        // We found a new parent for the node we're currently processing
                        // AKA a faster way
                        node.parent = cur;
                        node.g = tempG;
                        node.f = node.g + node.h;
                    }
                }
                else
                {
                    // Get the node's g, h, and f values
                    // Add the node to the open tile for later processing
                    node.parent = cur;
                    node.g = cur.g + Vector3.Distance(node.transform.position, cur.transform.position);
                    node.h = Vector3.Distance(node.transform.position, target.transform.position);
                    node.f = node.g + node.h;
                    open.Add(node);
                }
            }
        }

        // If we've reached here, then there is no legal path to take to the closest target.
        // For the sake of avoiding an infinite loop, we reduce this unit's action count by 1.
        GrabTargetNode(ReturnCurrentNode()); // Our new target node is our own node
        charMan.character.ReduceActionCount(); // Reduce the action count here
        this.enabled = false; // Turn the movement script off
        return;
    }

    /// <summary>
    /// FindLowestF() is used by the above FindPath() function to find which node in a 
    /// given list has the lowest F score out of all of them.
    /// </summary>
    /// <param name="open"> List to traverse </param>
    /// <returns> Reference to the node with the lowest F score </returns>
    private Node FindLowestF(List<Node> open)
    {
        // Save the very first node from the list in lowest
        Node lowest = open[0];

        // Using a for loop, check each node for a lower f cost than the current lowest
        for (int i = 1; i < open.Count; i++)
        {
            // If one is found, replace lowest with this new node
            if (open[i].f < lowest.f) { lowest = open[i]; }
        }

        // Remove the lowest f score node from the list and return it
        open.Remove(lowest);
        return lowest;
    }

    /// <summary>
    /// CalculateHCost() returns the calculated vector3 distance between the current node and the target
    /// and returns it
    /// </summary>
    /// <param name="current"> Node we're currently on </param>
    /// <param name="target"> Node we're hoping to reach </param>
    /// <returns></returns>
    private float CalculateHCost(Node current, Node target)
    {
        // Simple calculation of the distance between nodes' positions
        return Vector3.Distance(current.transform.position, target.transform.position);
    }

    /// <summary>
    /// FindEndNode() finds the node that's closest to our target node within this unit's
    /// own max move distance.
    /// </summary>
    /// <param name="node"> The current target node that we've found </param>
    /// <returns> Reference to the node that we can move to </returns>
    private Node FindEndNode(Node node)
    {
        // Create a new stack representing the path we'll take
        Stack<Node> tempPath = new Stack<Node>();

        // Grab the current node's parent node (as found by our A*) and add it to the list
        // All the while, go back through this list of parents until we have no more parent nodes
        Node next = node.parent;
        while (next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        // If the count of our temp path is less than or equal to our max move,
        // return the parent of the current node
        if (tempPath.Count <= maxMove) { return node.parent; }

        // endTile is the tile closest to the target
        Node endTile = null;
        // With a for loop, find the latest tile stored within and return it
        for (int i = 0; i <= maxMove; i++) { endTile = tempPath.Pop(); }
        return endTile;
    }
}
