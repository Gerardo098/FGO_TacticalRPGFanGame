using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerClickHandler
{
    enum Flag { consumable, servant }

    InventoryPanel itemPanel;
    ServantEquipPanel servantPanel;
    Flag flag;

    public void OnPointerClick(PointerEventData eventData)
    {
        // If we have a consumable item (ex. potion)
        if (flag == Flag.consumable)
        {
            if (itemPanel == null) { itemPanel = transform.parent.GetComponent<InventoryPanel>(); }
            itemPanel.OnInteract(transform.GetSiblingIndex());
        }
        // If we have a Servant
        else
        {
            if (servantPanel == null) { servantPanel = transform.parent.GetComponent<ServantEquipPanel>(); }
            servantPanel.OnInteract(transform.GetSiblingIndex());
        }
    }

    public void Set(ConsumableInstance consumable, InventoryPanel _itemPanel)
    {
        itemPanel = _itemPanel;
        flag = Flag.consumable;
        transform.GetChild(0).GetComponent<Text>().text = consumable.consumable.name + " " + consumable.count.ToString();
    }

    public void Set(ServantInstance servant, ServantEquipPanel _servantPanel)
    {
        servantPanel = _servantPanel;
        flag = Flag.servant;
        transform.GetChild(0).GetComponent<Text>().text = servant.servant.name;
    }
}
