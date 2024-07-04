using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private List<PlaceableObject> placeableObjects = new List<PlaceableObject>();
    [SerializeField] private GameObject inventoryGUI;
    [SerializeField] private GameObject inventoryGrid;
    [SerializeField] private GameObject inventoryItemPrefab;

    private PlaceableObject heldObject;
    public PlaceableObject HeldObject { get { return heldObject; } set { heldObject = value; } }
    private int tabIndex;
    private List<GameObject> gridObjectList = new List<GameObject>();
    private List<PlaceableObject> inventoryObjectList = new List<PlaceableObject>();

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

        Cursor.lockState = inventoryGUI.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        SwitchTab(tabIndex);
    }

    public void SwitchTab(int index)
    {
        tabIndex = index;
        Debug.Log($"New Tab Index is {index}");
        SetInventoryContent();
    }

    private void SetInventoryContent()
    {
        gridObjectList.ForEach(gridItem => Destroy(gridItem));
        gridObjectList.Clear();
        inventoryObjectList.Clear();

        int indexCount = 0;

        placeableObjects.ForEach(placeableObject => {
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
        Debug.Log($"Item clicked is {inventoryObjectList[index].name}");
    }
}
