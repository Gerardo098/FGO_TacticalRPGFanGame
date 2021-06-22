using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Consumable is a class representing one-use items like potions and scrolls
/// </summary>
[CreateAssetMenu(menuName = "Data/Consumable")]
public class Consumable : ItemBase
{
    public int sell, buy; // Sell and buy price for the item, respectively
}
