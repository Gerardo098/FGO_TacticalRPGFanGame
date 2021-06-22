using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Inventory inventory;
    public Equipment equipment;
    public Character_Root character;

    public void Init(Character_Root curCharacter)
    {
        character = curCharacter;
        inventory = curCharacter.GetComponent<Inventory>();
        equipment = curCharacter.GetComponent<Equipment>();
    }
}
