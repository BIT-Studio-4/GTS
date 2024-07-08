using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;

    // The list of all items that are purchasable
    [SerializeField] private List<StoreItemSO> allStoreItems = new List<StoreItemSO>();
    [SerializeField] private GameObject storeGUI;
    [SerializeField] private GameObject storeGrid;
    [SerializeField] private GameObject storeItemPrefab;
    private int tabIndex;
    private List<GameObject> gridObjectDisplayList = new List<GameObject>();
    // List of items currently displayed in GUI
    private List<StoreItemSO> storeObjectDisplayList = new List<StoreItemSO>();

    private void Awake()
    { 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InputSystem.actions.FindAction("ToggleStore").performed += ctx => ToggleStoreGUI();
        SwitchTab(0);
    }

    // This toggles the state of the Store GUI (open or closed)
    private void ToggleStoreGUI()
    {
        storeGUI.SetActive(!storeGUI.activeSelf);

        if (storeGUI.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            SwitchTab(tabIndex);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Sets all of the content of the store GUI
    private void SetStoreDisplayContent()
    {
        // Removes all the old GUI display gridItems, and clears the lists of what was in them
        gridObjectDisplayList.ForEach(gridItem => Destroy(gridItem));
        gridObjectDisplayList.Clear();
        storeObjectDisplayList.Clear();

        int indexCount = 0;

        // Iterates over all items to see if it should display in current tab
        allStoreItems.ForEach(placeableObject => {
            if (((int)placeableObject.type) == tabIndex)
            {
                CreateGridItem(indexCount, placeableObject);

                indexCount++;
            }
        });
    }

    // Instantiates a new grid GameObject in the Inventory menu
    private void CreateGridItem(int index, StoreItemSO storeItem)
    {
        // Creates the new GameObject and puts it in a list
        GameObject gridItem = Instantiate(storeItemPrefab, storeGrid.transform);
        gridObjectDisplayList.Add(gridItem);
        storeObjectDisplayList.Add(storeItem);
        InventoryItemSlot gridSlot = gridItem.GetComponent<InventoryItemSlot>();

        gridSlot.Button.onClick.AddListener(() => ItemButtonClick(index));
        gridSlot.Text.text = storeItem.name;
    }

    public void ItemButtonClick(int index)
    {
        StoreItemSO storeItem = storeObjectDisplayList[index];
    }

    // Changes the tab and resets the contents of the store
    public void SwitchTab(int index)
    {
        tabIndex = index;
        SetStoreDisplayContent();
    }
}
