using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Instance of {this} already exists, removing {this} on {gameObject}");
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void LoadSaveGame(SaveGame saveGame)
    {
        Debug.Log("Loading found savegame for current user");

        // Loading players money
        GameManager.Instance.Money = saveGame.money;

        // Loading items in inventory
        ClearInventoryItems();
        SetInventoryItems(new List<InventoryItem>(saveGame.inventory.items));

        // Loading objects in store
    }

    /// <summary>
    /// Clears all the items in the inventory
    /// </summary>
    private void ClearInventoryItems()
    {
        InventoryManager.Instance.InventoryPlaceableObjects.Clear();
    }

    /// <summary>
    /// Sets all the items in the inventory
    /// </summary>
    private void SetInventoryItems(List<InventoryItem> items)
    {
        foreach (InventoryItem item in items)
        {
            StoreItemSO storeItem = StoreManager.Instance.AllStoreItems.Find(itemSO => itemSO.id == item.item_id);

            InventoryManager.Instance.InventoryPlaceableObjects.Add(new PlaceableObject(storeItem.name, storeItem.id, storeItem, storeItem.prefab, storeItem.type, item.quantity));
        }
    }

    /// <summary>
    /// Clears all the objects in the store
    /// </summary>
    private void ClearStoreObjects()
    {

    }

    /// <summary>
    /// Sets all the objects in the store
    /// </summary>
    private void SetStoreObjects(List<StoreObject> objects)
    {
        
    }


}
