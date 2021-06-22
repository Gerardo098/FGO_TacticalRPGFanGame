using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ServantEquipPanel is a subclass of the ItemPanel class.
/// Servants aligned to the player character appear here when available, and may be switched
/// between them by clicking on their button.
/// </summary>
public class ServantEquipPanel : ItemPanel
{
    // List of all the buttons created in the UseItem panel
    List<ItemButton> buttons;

    /*
     * OnEnable() handles the order of operations upon accessing the UseItem panel
     */
    private void OnEnable()
    {
        // First thing's first, if we have no buttons list, initiate it
        if (buttons == null)
        {
            buttons = new List<ItemButton>();
            inventoryManager.equipment.onChange += UpdatePanel;
        }
        UpdatePanel(); // Call UpDatePanel() to handle button creation
    }

    /*
     * In UpdatePanel(), we generate new buttons for the UseItem panel.
     * One button per "stack" of Item-type item in our inventory.
     */
    public override void UpdatePanel()
    {
        // Remove any buttons left over from the previous function call
        while (buttons.Count > inventoryManager.inventory.GetServantCount)
        {
            // Destroy the button from the scene first, then remove it from our list
            Destroy(buttons[0].gameObject);
            buttons.RemoveAt(0);
        }

        // Instantiating and preparing buttons for our UseItem panel
        // Iterate through all items in the inventory
        for (int i = 0; i < inventoryManager.inventory.GetServantCount; i++)
        {
            // If we're low on buttons, make more
            if (i >= buttons.Count)
            {
                GameObject newButton = Instantiate(buttonPrefab, transform);
                buttons.Add(newButton.GetComponent<ItemButton>());
            }
            // Set the servant into our button
            buttons[i].Set(inventoryManager.inventory.GetServant(i), this);
        }
    }

    /*
     * OnInteract() in the ServantEquipPanel grabs the Servant we want to equip and throws it on - EZPZ
     */
    public override void OnInteract(int ID)
    {
        ServantInstance newServant = inventoryManager.inventory.GetServant(ID);
        Servant prevServant = inventoryManager.equipment.curServant.equippedServant;

        if (prevServant != null) // Check that the prev servant exists
        {
            // TODO - Function to subtract servant's equipment
            inventoryManager.character.parameterContainer.Subtract(prevServant.stats);
            // Send the previously equipped Servant back to the inventory
            inventoryManager.inventory.AddItem(prevServant);
            inventoryManager.equipment.UnequipGear();
        }

        // Add stats
        inventoryManager.character.parameterContainer.Sum(newServant.servant.stats);
        // Equip Servant - also, equip weapons + armour (if any)
        inventoryManager.equipment.EquipServant(newServant.servant);
        // Take the abilities + spells the Servant has and give them to the character
        inventoryManager.character.charMan.skillController.GrabActiveAbilities(newServant.servant);
        inventoryManager.character.charMan.unitEffects.GrabServantPassives(newServant.servant);
        // Remove the Servant from the inventory
        inventoryManager.inventory.RemoveItem(newServant.servant);

        // Update the panel
        UpdatePanel();
        // Equipping a Servant uses a turn!
        inventoryManager.character.ReduceActionCount();
    }

    public override void Show()
    {
        throw new System.NotImplementedException();
    }
}
