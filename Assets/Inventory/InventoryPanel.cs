using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * InventoryPanel works exclusively with the items we have (ex. potions, antidotes, burn heals, whatever)
 * and is tied to the item panel GameObject
 */
public class InventoryPanel : ItemPanel
{
    List<ItemButton> buttons;

    public override void OnInteract(int ID)
    {

        ConsumableInstance item = inventoryManager.inventory.GetItem2(ID); // Find the item in question
        // Function to get the item's effect
        Debug.Log("The unit uses: x1 " + item.consumable.name);
        inventoryManager.inventory.RemoveItem(item.consumable);
        UpdatePanel();
            /*
            ConsumableInstance item = inventoryManager.inventory.GetItem2(ID); // Find the item in question
            if (inventoryManager.equipment.CheckAvailableSlots(item)) // CHeck if it'll even go in an equipment slot
            {
                Item previousItem = inventoryManager.equipment.GetItemSlot(item.itemBase.itemType);
                // Check if that item is not null; if not, add it to inventory
                if (previousItem != null)
                {
                    // Remove the item's stats (if any) from the character's stats
                    inventoryManager.character.parameterContainer.Subtract(previousItem.stats);
                    inventoryManager.inventory.AddItem(previousItem);
                }

                // Add the item's stats to the character's stats
                inventoryManager.character.parameterContainer.Sum(item.itemBase.stats);
                inventoryManager.equipment.Equip(item.itemBase); // Add the item to an item slot
                // Remove the item from the inventory (cause we're wearing it now)
                inventoryManager.inventory.RemoveItem(item.itemBase);
                UpdatePanel();
            }
            */
    }

    /*
     * Show() instantiates and sets all buttons in the item panel.
     * Due to the redundancy between this and UpdatePanel(), Show() is simply here to cover the "override"
     */
    public override void Show()
    {
        /*
        List<ItemInstance> items = inventoryManager.inventory.GetInventory();

        if (items == null)
        {
            Debug.LogError("items = null; No inventory found.");
        }
        else
        {
            //Debug.Log(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                GameObject newButton = Instantiate(buttonPrefab, transform);
                newButton.GetComponent<ItemButton>().Set(items[i], this);
            }
        }
        */
    }

    /*
     * UpdatePanel(), when called, creates a group of buttons, sets their values, and 
     */
    public override void UpdatePanel()
    {
        // Remove unnecessary buttons
        while (buttons.Count > inventoryManager.inventory.GetItemCount)
        {
            Destroy(buttons[0].gameObject);
            buttons.RemoveAt(0);
        }

        // Add buttons to the InventoryPanel
        for (int i = 0; i < inventoryManager.inventory.GetItemCount; i++)
        {
            if (i >= buttons.Count)
            {
                GameObject newButton = Instantiate(buttonPrefab, transform);
                buttons.Add(newButton.GetComponent<ItemButton>());
            }
            buttons[i].Set(inventoryManager.inventory.GetItem2(i), this);
        }
    }

    private void OnEnable()
    {
        if (buttons == null) { buttons = new List<ItemButton>(); }
        UpdatePanel();
    }
}
