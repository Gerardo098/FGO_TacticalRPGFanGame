using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * UseItemPanel is an item panel class that handles operations in our UseItem panel (as the name implies),
 * accessable via the turn menu + Item button.
 * This works almost the same as all other panels used by the inventory.
 */
public class UseItemPanel : ItemPanel
{
    // List of all the buttons created in the UseItem panel
    List<ItemButton> buttons;

    /*
     * OnEnable() handles the order of operations upon accessing the UseItem panel
     */
    private void OnEnable()
    {
        // First thing's first, if we have no buttons list, initiate it
        if (buttons == null) { buttons = new List<ItemButton>(); }
        UpdatePanel(); // Call UpDatePanel() to handle button creation
    }

    /*
     * In UpdatePanel(), we generate new buttons for the UseItem panel.
     * One button per "stack" of Item-type item in our inventory.
     */

    public override void UpdatePanel()
    {
        /*
        // Remove any buttons left over from the previous function call
        while (buttons.Count > inventoryManager.inventory.GetItemCount)
        {
            // Destroy the button from the scene first, then remove it from our list
            Destroy(buttons[0].gameObject);
            buttons.RemoveAt(0);
        }

        // Instantiating and preparing buttons for our UseItem panel
        // Iterate through all items in the inventory
        for (int i = 0; i < inventoryManager.inventory.GetItemCount; i++)
        {
            // Check whether the item is an "item" and not another ItemType
            if(inventoryManager.inventory.GetItem(i).GetItemType() == ItemType.Item)
            {
                // If we're missing a button for an item, create it
                if (i >= buttons.Count)
                {
                    GameObject newButton = Instantiate(buttonPrefab, transform);
                    buttons.Add(newButton.GetComponent<ItemButton>());
                }
                // Then set the item into our button
                //buttons[i].Set(inventoryManager.inventory.GetItem(i), this);
            }
        }
        */
    }

    /*
     * The following functions are found in the ItemPanel class but are otherwise unused by this one
     */
    public override void OnInteract(int ID)
    {
        throw new System.NotImplementedException();
    }

    public override void Show()
    {
        throw new System.NotImplementedException();
    }
}
