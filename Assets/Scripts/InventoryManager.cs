using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

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
    [SerializeField] private GameObject selectOnOpen;
    //list of tabs at the top of the menu
    [SerializeField] private List<GameObject> tabs;
    private List<Image> tabShadows = new();
    private List<Image> tabBackgrounds = new();
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
    // List of the GameObjects which display the items in the inventory
    private List<GameObject> gridObjectDisplayList = new List<GameObject>();
    // List of items currently displayed in GUI
    private List<PlaceableObject> inventoryObjectDisplayList = new List<PlaceableObject>();
    // The GameObject that the player is holding for display.
    private GameObject playerHeldItem;
    //list of all button's animation triggers to allow mouse and controller button hover effects
    private List<AnimationTriggers> allButtonsAnimationTrigs = new();

    /// <summary>
    /// Makes InventoryManager a singleton
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Multiple instances InventoryManager found, deleting instance attached to {gameObject}");
            Destroy(this);
            return;
        }

        Instance = this;

        SetUpTabImages();
    }

    /// <summary>
    /// Sets inventory to be closed when the game starts
    /// </summary>
    void Start()
    {
        HeldObject = null;
        SwitchTab(0);

        SetUpAllButtonsAnimationsList();

        Debug.Log(shelfItem.itemName);
        // Adds a shelf to the inventory on start
        InventoryPlaceableObjects.Add(new PlaceableObject(shelfItem.itemName, shelfItem.id, shelfItem, shelfItem.prefab, shelfItem.type, 1));
    }

    /// <summary>
    /// This toggles the state of the Inventory GUI (open or closed)
    /// </summary>
    /// <param name="isActive">What you want to set the inventory open state to</param>
    public void SetInventoryActiveState(bool isActive)
    {
        inventoryGUI.SetActive(isActive);

        if (inventoryGUI.activeSelf) //active
        {
            InputDeviceManager.Instance.onGameDeviceChanged.AddListener(HandleInputDeviceType);
            SwitchTab(tabIndex);
            ClearHandItem();
            HandleInputDeviceType(); //set first selected if gamepad
            if (TutorialManager.Instance.InProgress) TutorialManager.Instance.CompleteTutorialTask("openedInventory");
        }
        else //not active
        {
            InputDeviceManager.Instance.onGameDeviceChanged.RemoveListener(HandleInputDeviceType);
        }
    }

    /// <summary>
    /// Changes the tab and resets the contents of the inventory
    /// </summary>
    /// <param name="index">The index of the tab you want to switch to</param>
    public void SwitchTab(int index)
    {
        tabIndex = index;
        SetInventoryDisplayContent();
        ChangeTabColours();
    }

    private void SetUpTabImages()
    {
        foreach (GameObject tab in tabs)
        {
            Image[] allImages = tab.GetComponentsInChildren<Image>();
            tabBackgrounds.Add(allImages[0]); //background is parent, always first
            tabShadows.Add(allImages[allImages.Length - 1]); //shadows are always last child image
        }
    }

    private void ChangeTabColours()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (i == tabIndex) //active tab
            {
                tabBackgrounds[i].color = UIStyling.Instance.TabSelectedColor;
                tabShadows[i].enabled = false;
            }
            else //non-active tab(s)
            {
                tabBackgrounds[i].color = UIStyling.Instance.ButtonDeselectedColor;
                tabShadows[i].enabled = true;
            }
        }
    }

    /// <summary>
    /// Sets all of the content of the inventory GUI
    /// </summary>
    private void SetInventoryDisplayContent()
    {
        // Removes all the old GUI display gridItems, and clears the lists of what was in them
        gridObjectDisplayList.ForEach(gridItem => Destroy(gridItem));
        gridObjectDisplayList.Clear();
        inventoryObjectDisplayList.Clear();

        int indexCount = 0;

        // Iterates over all stock to see if it should display in current tab
        inventoryPlaceableObjects.ForEach(placeableObject =>
        {
            if (((int)placeableObject.type) == tabIndex)
            {
                CreateGridItem(indexCount, placeableObject);

                indexCount++;
            }
        });
    }

    /// <summary>
    /// Instantiates a new grid item GameObject in the Inventory menu
    /// </summary>
    /// <param name="index"></param>
    /// <param name="placeableObject"></param>
    private void CreateGridItem(int index, PlaceableObject placeableObject)
    {
        // Creates the new GameObject and puts it in a list
        GameObject gridItem = Instantiate(inventoryItemPrefab, inventoryGrid.transform);
        gridObjectDisplayList.Add(gridItem);
        inventoryObjectDisplayList.Add(placeableObject);
        InventoryItemSlot gridSlot = gridItem.GetComponent<InventoryItemSlot>();

        // Assigns the information to a certain grid item
        gridSlot.Button.onClick.AddListener(() => StockButtonClick(index));
        gridSlot.Text.text = placeableObject.name;
        gridSlot.CountText.text = $"{placeableObject.count}x";

        if (placeableObject.prefab.TryGetComponent(out SellItem randomSell))
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

        //add button to all button animation triggers list
        allButtonsAnimationTrigs.Add(gridSlot.Button.GetComponent<Button>().animationTriggers);
    }

    /// <summary>
    /// Collect all the animation triggers for all buttons into one list
    /// </summary>
    private void SetUpAllButtonsAnimationsList()
    {
        foreach (GameObject tab in tabs)
        {
            allButtonsAnimationTrigs.Add(tab.GetComponentInChildren<Button>().animationTriggers);
        }
        //inventoryItemSlot buttons are setup in CreateGridItem()
    }

    /// <summary>
    /// Method that is called when an item button is clicked
    /// </summary>
    /// <param name="index"></param>
    public void StockButtonClick(int index)
    {
        PlaceableObject placeableObject = inventoryObjectDisplayList[index];

        if (placeableObject.count <= 0) return;

        SetHandItem(placeableObject);

        // This is done via UI manager so the correct windows are opened and closed
        UIManager.Instance.SetGUIState(UIType.Inventory, false);
    }

    /// <summary>
    /// Sets the item the player is holding for display purposes
    /// </summary>
    /// <param name="placeableObject"></param>
    private void SetHandItem(PlaceableObject placeableObject)
    {
        ClearHandItem();

        playerHeldItem = Instantiate(placeableObject.prefab, handDisplayItemParent.transform);
        handDisplayItemParent.transform.localScale = ((int)placeableObject.type) == 0 ? new Vector3(stockHandScale, stockHandScale, stockHandScale) : new Vector3(structureHandScale, structureHandScale, structureHandScale);
        SellItem randomSell = playerHeldItem.GetComponent<SellItem>();
        if (randomSell != null)
            randomSell.enabled = false;

        HeldObject = placeableObject;

        if (TutorialManager.Instance.InProgress && HeldObject.id == 4) // checks if player picked up shelf
        {
            TutorialManager.Instance.CompleteTutorialTask("selectedShelf");
        }
    }

    /// <summary>
    /// Clears the item the player is holding for display purposes
    /// </summary>
    private void ClearHandItem()
    {
        if (playerHeldItem == null) return;

        Destroy(playerHeldItem);
        playerHeldItem = null;

        HeldObject = null;
    }

    /// <summary>
    /// Consumes an item from the inventory when it is placed
    /// </summary>
    public void ConsumePlacedItem()
    {
        HeldObject.count -= 1;

        if (HeldObject.count <= 0)
        {
            ClearHandItem();
        }
    }

    private void HandleInputDeviceType()
    {
        if (InputDeviceManager.Instance.ActiveDevice == InputDevice.KeyboardMouse)
        {
            foreach (AnimationTriggers trigs in allButtonsAnimationTrigs)
            {
                trigs.highlightedTrigger = "Highlighted";
                trigs.selectedTrigger = "Normal";
            }
            UIManager.Instance.EventSystemMain.SetSelectedGameObject(null);
        }
        else if (InputDeviceManager.Instance.ActiveDevice == InputDevice.Gamepad)
        {
            foreach (AnimationTriggers trigs in allButtonsAnimationTrigs)
            {
                trigs.highlightedTrigger = "Normal";
                trigs.selectedTrigger = "Highlighted";
            }
            UIManager.Instance.EventSystemMain.SetSelectedGameObject(selectOnOpen);
        }
    }
}
