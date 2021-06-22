using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ItemBase is an abstract class that all other items and equipment 
/// </summary>
public abstract class ItemBase : ScriptableObject
{
    public new string name; // item name
    public Rank rank; // item rank; strength
    public List<string> tags; // descriptive tags
}
