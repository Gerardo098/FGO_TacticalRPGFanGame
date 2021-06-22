using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * EquippedItemsPanel is the class that handles 
 * This works almost the same as all other panels used by the inventory.
 */
public class EquippedItemsPanel : ItemPanel
{
    List<ItemButton> itemButtons;

    private void OnEnable()
    {
        if (itemButtons == null)
        {
            itemButtons = new List<ItemButton>();
            Show();
            inventoryManager.equipment.onChange += UpdatePanel;
        }
        UpdatePanel();
    }

    public override void OnInteract(int ID)
    {
        
    }

    public override void UpdatePanel()
    {
        for (int i = 0; i < itemButtons.Count; i++)
        {
            //itemButtons[i].Set(inventoryManager.equipment.equipmentSlots[i].equipped, this);
        }
    }

    public override void Show()
    {
        throw new System.NotImplementedException();
    }
}
