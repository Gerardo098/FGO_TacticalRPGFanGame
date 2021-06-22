using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Node class is used by individual tiles or "nodes" that make up the scene's grid map.
/// This class uses A* pathfinding and stores variables necessary for it here.
/// </summary>
/*
 * Script for individual tiles or "nodes" on the game's grid map.
 * We use the A* style of pathfinding for this game
 */

public class Node : MonoBehaviour
{
    // Variables
    public bool walkable = true; // The node may be walked upon by a character

    public bool current = false; // Current when it is the tile we're standing on or the "source" tile
    public bool target = false; // Target when it is selected as our "end" tile
    public bool selectable = false; // Selectable when a tile is within range

    public List<Node> adjacents = new List<Node>(); // List of all nodes adjacent to this node

    public Node parent = null; // Node that "leads" to this node
    public bool visited = false;
    public int distance = 0;

    // A*
    public float f = 0;
    public float g = 0;
    public float h = 0;

    /// <summary>
    /// Update(), change the colour of the node in accordance to their flags
    /// </summary>
    private void Update()
    {
        // Current tile is purple; target is red; selectable are green; others are white
        if (!walkable) { GetComponent<Renderer>().material.color = Color.black; }
        else if (current) { GetComponent<Renderer>().material.color = Color.magenta; }
        else if (target) { GetComponent<Renderer>().material.color = Color.red; }
        else if (selectable) { GetComponent<Renderer>().material.color = Color.green; }
        else { GetComponent<Renderer>().material.color = Color.white; }
    }

    public void CalculateF() { f = g + h; }

    /// <summary>
    /// Reset() returns all of a node's variables to their initial, default state
    /// </summary>
    public void Reset()
    {
        adjacents.Clear();
        current = false;
        target = false;
        selectable = false;
        visited = false;
        parent = null;
        distance = 0;
        f = g = h = 0; // Set the A* variables to 0
    }

    /// <summary>
    /// FindNeighbours() finds all neighbours of a given node, in each of the 4 cardinal directions;
    /// ex. north, east, south, west.
    /// </summary>
    /// <param name="jumpHeight"> How high a node can be to be adjacent </param>
    /// <param name="target"> Node to check the neighbours of </param>
    public void FindNeighbours(float jumpHeight, Node target)
    {
        // Clear the node first from anything as to not confuse the code
        Reset();

        // Check the tiles
        CheckNode(Vector3.right, jumpHeight, target); // Right
        CheckNode(-Vector3.right, jumpHeight, target); // Left
        CheckNode(Vector3.forward, jumpHeight, target); // Up
        CheckNode(-Vector3.forward, jumpHeight, target); // Down

    }

    /// <summary>
    /// CheckNode() explicitly finds the neighbour adjacent to the target node in a given direction
    /// </summary>
    /// <param name="direction"> vector3 direction to look towards </param>
    /// <param name="jumpHeight"> How high a node can be to be adjacent </param>
    /// <param name="target"> Node to check the neighbour of </param>
    private void CheckNode(Vector3 direction, float jumpHeight, Node target)
    {
        // Y in halfExtents is used to calculate the jump height of our character's 
        Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider c in colliders)
        {
            // Grab the node from the collider we hit
            Node node = c.GetComponent<Node>();
            // If the node is not null and it's a walkable node
            if (node != null && node.walkable)
            {
                RaycastHit hit;
                if (!Physics.Raycast(node.transform.position, Vector3.up, out hit, 1) || (node == target))
                {
                    adjacents.Add(node);
                }
            }
        }
    }


}
