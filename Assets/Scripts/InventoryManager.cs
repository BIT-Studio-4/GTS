using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private GameObject inventoryGUI;
    public GameObject InventoryGUI { get => inventoryGUI; set => inventoryGUI = value; }
    [SerializeField] private GameObject inventoryGrid;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject playerHeldItemParent;
    [SerializeField] private float stockScale;
    [SerializeField] private float structureScale;

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
        }
    }
    private int tabIndex;
    private List<GameObject> gridObjectDisplayList = new List<GameObject>();
    // List of items currently displayed in GUI
    private List<PlaceableObject> inventoryObjectDisplayList = new List<PlaceableObject>();
    // The GameObject that the player is holding for display.
    private GameObject playerHeldItem;

    // Makes InventorManager a singleton
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // Sets inventory to be closed when the game starts
    void Start()
    {
        HeldObject = null;
        SwitchTab(0);
    }
    
    // This toggles the state of the Inventory GUI (open or closed)
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

        playerHeldItem = Instantiate(placeableObject.prefab, playerHeldItemParent.transform);
        playerHeldItemParent.transform.localScale = ((int)placeableObject.type) == 0 ? new Vector3(stockScale, stockScale, stockScale) : new Vector3(structureScale, structureScale, structureScale);
        RandomSell randomSell = playerHeldItem.GetComponent<RandomSell>();
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
