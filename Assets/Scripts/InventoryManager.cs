using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    //This will be the list that things are added and removed to via purchasing items
    [SerializeField] private List<PlaceableObject> inventoryPlaceableObjects = new List<PlaceableObject>();
    [SerializeField] private GameObject inventoryGUI;
    [SerializeField] private GameObject inventoryGrid;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject playerHeldItemParent;
    [SerializeField] private float stockScale;
    [SerializeField] private float structureScale;


    private PlaceableObject heldObject;
    public PlaceableObject HeldObject { get { return heldObject; } set { heldObject = value; } }
    private int tabIndex;
    private List<GameObject> gridObjectList = new List<GameObject>();
    private List<PlaceableObject> inventoryObjectList = new List<PlaceableObject>();
    private GameObject playerHeldItem;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        inventoryGUI.SetActive(false);
        HeldObject = null;
        InputSystem.actions.FindAction("ToggleInventory").performed += ctx => ToggleInventoryGUI();
        SwitchTab(0);
    }
    
    private void ToggleInventoryGUI()
    {
        inventoryGUI.SetActive(!inventoryGUI.activeSelf);

        if (inventoryGUI.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            SwitchTab(tabIndex);
            ClearHandItem();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Changes the tab and resets the contents of the inventory
    public void SwitchTab(int index)
    {
        tabIndex = index;
        SetInventoryContent();
    }

    // Sets all of the content of the inventory GUI
    private void SetInventoryContent()
    {
        gridObjectList.ForEach(gridItem => Destroy(gridItem));
        gridObjectList.Clear();
        inventoryObjectList.Clear();

        int indexCount = 0;

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
        gridObjectList.Add(gridItem);
        inventoryObjectList.Add(placeableObject);
        InventoryItemSlot gridSlot = gridItem.GetComponent<InventoryItemSlot>();

        gridSlot.Button.onClick.AddListener(() => StockButtonClick(index));
        gridSlot.Text.text = placeableObject.name;
    }

    // Method that is called when an item button is clicked
    public void StockButtonClick(int index)
    {
        PlaceableObject placeableObject = inventoryObjectList[index];

        SetHandItem(placeableObject);
        ToggleInventoryGUI();
    }

    private void SetHandItem(PlaceableObject placeableObject)
    {
        ClearHandItem();

        playerHeldItem = Instantiate(placeableObject.prefab, playerHeldItemParent.transform);
        playerHeldItemParent.transform.localScale = ((int)placeableObject.type) == 0 ? new Vector3(stockScale, stockScale, stockScale) : new Vector3(structureScale, structureScale, structureScale);
    }

    private void ClearHandItem()
    {
        Destroy(playerHeldItem);
        playerHeldItem = null;
    }
}
