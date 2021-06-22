using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public class Item : ScriptableObject
{
    public string Name; // Name of the item
    public int sell, buy; // Sell and buy price of the item, respecitvely
    public bool stackable; // Whether you can stack the item in your inventory (ex 1 - 99 potions)
    public Rank rank; // The conceptual "Rank" of the item, higher the rank, stronger the item

    //public ItemType itemType; // Type of item (armour, weapon, whatever)
    
    public ItemParameterContainer stats; // Stats associated with the item (mostly used for the player's Heroes)
    public ServantEquipment servantEquipment; // Abilities and gear possessed by the item (such as Servants' heroic abilities)
}
