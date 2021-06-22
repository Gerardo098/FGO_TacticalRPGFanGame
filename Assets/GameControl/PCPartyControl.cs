using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PCPartyControl is a class meant to control the player's party and inventory
/// inbetween combat scenes, such as selecting their party before a battle or
/// preparing the order of their Servants.
/// </summary>
public class PCPartyControl : MonoBehaviour
{

    // PC character information
    public GameObject PlayerCharacter;
    static public string PCName;

    // PC Inventory information
    static public List<Servant> playerServants; // Servants the PC has allied with
    static public List<Consumable> playerConsumables; // Consumables in the party's possession
    static public void AddToInventory(Servant servant) { playerServants.Add(servant); }
    static public void AddToInventory(Consumable item) { playerConsumables.Add(item); }
    static public void RemoveServant(Servant servant) { playerServants.Remove(servant); }
    static public void RemoveServant(int index) { playerServants.RemoveAt(index); }

    // PC Party information
    static public List<GameObject> party; // The player's party, including the player themselves

    /// <summary>
    /// AddToParty() saves a "Player" gameobject as a party member.
    /// </summary>
    /// <param name="unit"> game object to consider </param>
    static public void AddToParty(GameObject unit)
    {
        if (unit.tag == "Player") { party.Add(unit); } // If the object is a "player", it must be a unit
        // Else, tell the player we can't add this unit to the party
        else { Debug.Log("Unable to add - Unit is not a viable party member"); }
    }
    static public void RemoveFromParty(GameObject unit) { party.Remove(unit); }
    static public void RemoveFromParty(int index) { party.RemoveAt(index); }
}
