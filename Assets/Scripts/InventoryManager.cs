using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private GameObject inventoryGUI;
    public GameObject InventoryGUI { get => inventoryGUI; set => inventoryGUI = value; }
    // The grid that aligns the objects in the UI
    [SerializeField] private GameObject inventoryGrid;
    // The prefab for each item displayed in the UI
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject handDisplayItemParent;
    [SerializeField] private float stockHandScale;
    [SerializeField] private float structureHandScale;
    [SerializeField] private StoreItemSO shelfItem; // Reference to StoreItemSO for shelf

    // This is the list of items the inventory contains
    private List<PlaceableObject> inventoryPlaceableObjects = new List<PlaceableObject>();
    public List<PlaceableObject> InventoryPlaceableObjects { get => inventoryPlaceableObjects; set => inventoryPlaceableObjects = value; }

    // The data stored about each object that is held
    private PlaceableObject heldObject;
    public PlaceableObject HeldObject
    {
        get
        {
            if (heldObject == null)
                ClearHandItem();
            return heldObject;
        }
        set
        {
            heldObject = value;
            if (heldObject == null)
                ClearHandItem();
            OnHeldObjectChange?.Invoke(heldObject);
        }
    }
    public Action<PlaceableObject> OnHeldObjectChange;
    private int tabIndex;
    private List<GameObject> gridObjectDisplayList = new List<GameObject>();
    // List of items currently displayed in GUI
    private List<PlaceableObject> inventoryObjectDisplayList = new List<PlaceableObject>();
    // The GameObject that the player is holding for display.
    private GameObject playerHeldItem;

    /// <summary>
    /// Makes InventoryManager a singleton
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Sets inventory to be closed when the game starts
    /// </summary>
    void Start()
    {
        HeldObject = null;
        SwitchTab(0);

        Debug.Log(shelfItem.itemName);
        // Adds a shelf to the inventory on start
        InventoryPlaceableObjects.Add(new PlaceableObject(shelfItem.itemName, shelfItem.prefab, shelfItem.type, 1));
    }

    /// <summary>
    /// This toggles the state of the Inventory GUI (open or closed)
    /// </summary>
    /// <param name="isActive"></param>
    public void SetInventoryActiveState(bool isActive)
    {
        inventoryGUI.SetActive(isActive);

        if (inventoryGUI.activeSelf)
        {
            SwitchTab(tabIndex);
            ClearHandItem();
        }
    }

    // Changes the tab and resets the contents of the inventory
    public void SwitchTab(int index)
    {
        tabIndex = index;
        SetInventoryDisplayContent();
    }

    // Sets all of the content of the inventory GUI
    private void SetInventoryDisplayContent()
    {
        // Removes all the old GUI display gridItems, and clears the lists of what was in them
        gridObjectDisplayList.ForEach(gridItem => Destroy(gridItem));
        gridObjectDisplayList.Clear();
        inventoryObjectDisplayList.Clear();

        int indexCount = 0;

        // Iterates over all stock to see if it should display in current tab
        inventoryPlaceableObjects.ForEach(placeableObject => {
            if (((int)placeableObject.type) == tabIndex)
            {
                CreateGridItem(indexCount, placeableObject);

                indexCount++;
            }
        });
    }

    // Instantiates a new grid GameObject in the Inventory menu
    private void CreateGridItem(int index, PlaceableObject placeableObject)
    {
        // Creates the new GameObject and puts it in a list
        GameObject gridItem = Instantiate(inventoryItemPrefab, inventoryGrid.transform);
        gridObjectDisplayList.Add(gridItem);
        inventoryObjectDisplayList.Add(placeableObject);
        InventoryItemSlot gridSlot = gridItem.GetComponent<InventoryItemSlot>();

        gridSlot.Button.onClick.AddListener(() => StockButtonClick(index));
        gridSlot.Text.text = placeableObject.name;
        gridSlot.CountText.text = $"{placeableObject.count}x";

        if (placeableObject.prefab.TryGetComponent<SellItem>(out SellItem randomSell))
        {
            // If the component exists, set the sale price
            float salePrice = randomSell.moneyOnSell;
            gridSlot.SalePriceText.text = $"${salePrice:F2}";
        }
        else
        {
            // If the component does not exist, set the text to empty or a desired message
            gridSlot.SalePriceText.text = ""; 
        }
    }

    // Method that is called when an item button is clicked
    public void StockButtonClick(int index)
    {
        PlaceableObject placeableObject = inventoryObjectDisplayList[index];

        if (placeableObject.count <= 0) return;

        SetHandItem(placeableObject);

        // This is done via UI manager so the correct windows are opened and closed
        UIManager.Instance.SetGUIState(UIType.Inventory, false);
    }

    // Sets the item the player is holding
    private void SetHandItem(PlaceableObject placeableObject)
    {
        ClearHandItem();

        playerHeldItem = Instantiate(placeableObject.prefab, handDisplayItemParent.transform);
        handDisplayItemParent.transform.localScale = ((int)placeableObject.type) == 0 ? new Vector3(stockHandScale, stockHandScale, stockHandScale) : new Vector3(structureHandScale, structureHandScale, structureHandScale);
        SellItem randomSell = playerHeldItem.GetComponent<SellItem>();
        if (randomSell != null)
            randomSell.enabled = false;
        
        HeldObject = placeableObject;
    }

    // Clears the item the player is holding
    private void ClearHandItem()
    {
        if (playerHeldItem == null) return;

        Destroy(playerHeldItem);
        playerHeldItem = null;
        
        HeldObject = null;
    }

    // Consumes an item when it is placed
    public void ConsumePlacedItem()
    {
        HeldObject.count -= 1;

        if (HeldObject.count <= 0)
        {
            ClearHandItem();
        }
    }
}
