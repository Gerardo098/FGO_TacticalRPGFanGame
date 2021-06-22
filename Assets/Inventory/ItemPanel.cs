using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 */
public abstract class ItemPanel : MonoBehaviour
{
    // buttonPrefab prepared for all item related 
    public GameObject buttonPrefab;
    public InventoryManager inventoryManager;

    public abstract void OnInteract(int ID);
    public abstract void Show();
    public abstract void UpdatePanel();
}
