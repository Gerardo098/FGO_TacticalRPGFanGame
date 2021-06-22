using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ItemInstance class
 * ItemInstance acts like a container for our items. It helps count the # of that item that we have as well
 * as works in tandem with the inventory 
 */

public class ServantInstance
{
    public Servant servant;

    public ServantInstance(Servant _servant)
    {
        servant = _servant;
    }

    public Servant GetServant() { return servant; }
}

public class WeaponInstance
{
    public Weapon weapon;

    public WeaponInstance(Weapon _weapon)
    {
        weapon = _weapon;
    }
}

public class ConsumableInstance
{
    public Consumable consumable;
    public int count;

    public ConsumableInstance(Consumable _consumable, int _count = 1)
    {
        consumable = _consumable;
        count = _count;
    }
}

/*
 * ============================================================================================================
 * INVENTORY CLASS STARTS HERE
 * The Inventory class holds ItemInstances within it to represent what a character has on their person.
 * ============================================================================================================
 */
public class Inventory : MonoBehaviour
{

    List<ConsumableInstance> inventory; // Inventory too
    List<ServantInstance> servantList; // Servants available to the unit
    [SerializeField]
    List<Servant> servantsOnStart; // Servants the unit begins with
    [SerializeField]
    List<Consumable> consumablesOnStart; // Items the unit begins with

    /*
     * GetItemCount returns the # of Item "stacks" we have in our inventory.
     * This does not take how many of each item there are per stack
     * For example, 2 potions stack while 2 weapons are separate entitites in the inventory
     */
    public int GetItemCount
    {
        get { return inventory.Count; }
    }

    public int GetServantCount
    {
        get { return servantList.Count; }
    }

    public void Init()
    {
        // Initialize the inventory lists
        inventory = new List<ConsumableInstance>();
        servantList = new List<ServantInstance>();
        // Add all of our OnStart items into the inventory
        for (int i = 0; i < servantsOnStart.Count; i++) { AddItem(servantsOnStart[i]); }
        for (int i = 0; i < consumablesOnStart.Count; i++) { AddItem(consumablesOnStart[i]); }
    }

    /*
     * This version of AddItem() handles the unit gaining a new consumable item
     * Also checks if we already have the item
     */
    public void AddItem(Consumable consumable, int count = 1)
    {
        // Catches any nulls that try to sneak in
        if (consumable == null) { return; }
        // Consumables are stackable, so we have to check if we already have a copy in our inventory
        ConsumableInstance consumableInstance = inventory.Find(x => x.consumable == consumable);
        // If we do not have it, add it to the list
        if (consumableInstance == null)
        {
            inventory.Add(new ConsumableInstance(consumable, count));
        }
        // But if we do, just increase the amount of that item instead
        else
        {
            consumableInstance.count += count;
        }
    }

    /*
     * This version of AddItem() handles the unit gaining a new Servant
     */
    public void AddItem(Servant servant)
    {
        // Catches any nulls that try to sneak in
        if (servant == null) { return; }
        // Add the Servant to the list
        servantList.Add(new ServantInstance(servant));
    }

    public void RemoveItem(Consumable consumable, int count = 1)
    {
        ConsumableInstance consumableInstance = inventory.Find(x => x.consumable == consumable);
        if (consumableInstance == null) { return; }
        consumableInstance.count -= count;
        if (consumableInstance.count <= 0) { inventory.Remove(consumableInstance); }
    }

    public void RemoveItem(Servant servant)
    {
        ServantInstance servantInstance = servantList.Find(x => x.servant == servant);
        if (servantInstance == null) { return; }
        servantList.Remove(servantInstance);
    }

    /*
     * GetItem() grabs the item located in position iD in the inventory
     */
    internal ServantInstance GetServant(int iD) { return servantList[iD]; }
    internal ConsumableInstance GetItem2(int iD) { return inventory[iD]; }
}
