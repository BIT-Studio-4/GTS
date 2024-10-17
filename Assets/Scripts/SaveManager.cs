using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private GameObject placedObjects;
    // Minutes between auto saves
    [SerializeField, Range(1f, 20f)] private int autoSaveCooldown = 5;
    public int AutoSaveCooldown 
    { 
        get =>  autoSaveCooldown; 
        set 
        {
            autoSaveCooldown = value;
            StartAutoSave();
        }
    }

    private Coroutine autoSaveCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Debug.LogWarning($"Instance of SaveManager already exists, removing {this} on {gameObject}");
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(10f);

        SaveGame();
    }

    /// <summary>
    /// Saves the game to the API
    /// </summary>
    public void SaveGame()
    {
        SaveGame saveSnapshot = GetCurrentSave();

    }

    /// <summary>
    /// Creates a save from the current state of the game
    /// </summary>
    /// <returns></returns>
    private SaveGame GetCurrentSave()
    {
        SaveGame saveGame = new SaveGame()
        {
            id = GameManager.Instance.User.id,
            Money = GameManager.Instance.Money,
            store = new Store()
            {
                store_objects = GetStoreObjects(),
            },
            inventory = new Inventory()
            {
                items = GetInventoryItems(),
            }
        };

        return saveGame;
    }

    private StoreObject[] GetStoreObjects()
    {
        List<StoreObject> storeObjects = new List<StoreObject>();



        InventoryManager.Instance.InventoryPlaceableObjects.ForEach(placedObject =>
        {
            storeObjects.Add(new StoreObject()
            {
                item_id = placedObject.id,
                
            });
        });

        return storeObjects.ToArray();
    }

    private InventoryItem[] GetInventoryItems()
    {
        List<InventoryItem> inventoryItems = new List<InventoryItem>();

        InventoryManager.Instance.InventoryPlaceableObjects.ForEach(item =>
        {
            inventoryItems.Add(new InventoryItem()
            {
                item_id = item.id,
                quantity = item.count
            });
        });

        return inventoryItems.ToArray();
    }

    /// <summary>
    /// Loads the game from the api
    /// </summary>
    public void LoadGame()
    {

    }

    /// <summary>
    /// Loops every autoSaveCooldown minutes and saves the game
    /// </summary>
    public IEnumerator AutoSave(float cooldown)
    {
        while (true)
        {
            yield return new WaitForSeconds(cooldown);

            SaveGame();
        }
    }

    /// <summary>
    /// Stops the auto save coroutine if its already going and starts a new one
    /// </summary>
    private void StartAutoSave()
    {
        if (autoSaveCoroutine != null)
        {
            StopAutoSave();
        }

        autoSaveCoroutine = StartCoroutine(AutoSave(autoSaveCooldown));
    }

    /// <summary>
    /// Stops the auto save coroutine
    /// </summary>
    private void StopAutoSave()
    {
        StopCoroutine(autoSaveCoroutine);
    }
}
