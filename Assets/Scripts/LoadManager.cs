using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance { get; private set; }

    [SerializeField] private PlaceObject placeObject;

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
        Debug.Log($"Loading found savegame for {GameManager.Instance.User.name}");

        // Loading players money
        GameManager.Instance.Money = saveGame.money;

        // Loading items in inventory
        ClearInventoryItems();
        SetInventoryItems(new List<InventoryItem>(saveGame.inventory.items));

        // Loading objects in store
        ClearStoreObjects();
        SetStoreObjects(new List<StoreObject>(saveGame.store.store_objects));
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
        foreach(PlacedObject placedObject in StockManager.Instance.PlacedObjects)
        {
            Destroy(placedObject.gameObject);
        }

        StockManager.Instance.PlacedObjects.Clear();
    }

    /// <summary>
    /// Sets all the objects in the store
    /// </summary>
    private void SetStoreObjects(List<StoreObject> objects)
    {
        foreach(StoreObject storeObject in objects)
        {
            StoreItemSO storeItem = StoreManager.Instance.AllStoreItems.Find(itemSO => itemSO.id == storeObject.item_id);

            placeObject.InstantiateObject
            (
                new PlaceableObject(storeItem.name, storeItem.id, storeItem, storeItem.prefab, storeItem.type, 1),
                new Vector3(storeObject.x_pos, storeObject.y_pos, storeObject.z_pos),
                Quaternion.Euler(new Vector3(0, storeObject.y_rot, 0)),
                placeObject.PlacedObjectsParent.transform
            );
        }
    }
}
