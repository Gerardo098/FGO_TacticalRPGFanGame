using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// We building tool scripts now

public class MenuScript
{

    // Function for assigning the tile material to all objects possessing the tag "Tile" in the scene
    [MenuItem("Tools/Assign Node Material")]
    public static void AssignTileMaterial()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        Material material = Resources.Load<Material>("Tile");

        foreach (GameObject t in nodes)
        {
            t.GetComponent<Renderer>().material = material;
        }
    }

    [MenuItem("Tools/Assign Node Script")]
    public static void AssignTileScript()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");

        foreach (GameObject t in nodes)
        {
            t.AddComponent<Node>();
        }
    }

    [MenuItem("Tools/Assign Node Tag")]
    public static void TileToNode()
    {
        GameObject[] soonToBeNodes = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject t in soonToBeNodes)
        {
            t.tag = "Node";
        }
    }
}
