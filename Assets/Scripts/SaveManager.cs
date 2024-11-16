using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private GameObject placedObjects;
    // Minutes between auto saves
    [SerializeField, Range(0f, 20f)] private int autoSaveCooldown = 5;

    private int autoSaveCheck;

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

    private void Start()
    {
        autoSaveCheck = autoSaveCooldown;
        StartAutoSave();
    }

    // Used to change the autosave loop when its updated in the inspector
    private void OnValidate()
    {
        if (autoSaveCooldown != autoSaveCheck && Application.isPlaying)
        {
            autoSaveCheck = autoSaveCooldown;
            if (autoSaveCooldown >= 1) StartAutoSave();
            else StopAutoSave();
        }
    }

    /// <summary>
    /// Saves the game to the API
    /// </summary>
    public void SaveGame()
    {
        if (GameManager.Instance.User is null)
        {
            Debug.LogWarning("User does not exist! Skipping save.");
            return;
        } 

        SaveGame saveSnapshot = GetCurrentSaveState();
        ApiManager.Instance.CreateSaveGame($"{ApiManager.Instance.ApiUrl}/api/save_games", saveSnapshot, GameManager.Instance.User);

        HUDManager.Instance.SaveGame();
    }

    /// <summary>
    /// Creates a save from the current state of the game
    /// </summary>
    /// <returns></returns>
    private SaveGame GetCurrentSaveState()
    {
        SaveGame saveGame = new SaveGame()
        {
            id = GameManager.Instance.User.id,
            money = GameManager.Instance.Money,
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

        StockManager.Instance.PlacedObjects.ForEach(placedObject =>
        {
            Vector3 pos = placedObject.transform.position;

            storeObjects.Add(new StoreObject()
            {
                item_id = placedObject.StoreItem.id,
                x_pos = pos.x,
                y_pos = pos.y,
                z_pos = pos.z,
                y_rot = placedObject.transform.rotation.eulerAngles.y,
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
    /// Loops every autoSaveCooldown minutes and saves the game
    /// </summary>
    public IEnumerator AutoSave(int cooldown)
    {
        // 60 * so its in minutes
        int minuteLengthInSeconds = 60;
        int secondsTime = cooldown * minuteLengthInSeconds;

        while (true)
        {
            yield return new WaitForSeconds(secondsTime);

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
