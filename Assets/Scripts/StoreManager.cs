using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;

    // The list of all items that are purchasable
    [SerializeField] private List<StoreItemSO> allStoreItems = new List<StoreItemSO>();
    public List<StoreItemSO> AllStoreItems { get => allStoreItems; }
    [SerializeField] private GameObject storeGUI;
    public GameObject StoreGUI { get => storeGUI; set => storeGUI = value; }
    // The grid that aligns the objects in the UI
    [SerializeField] private GameObject storeGrid;
    // The prefab for each item displayed in the UI
    [SerializeField] private GameObject storeItemPrefab;
    [SerializeField] private TextMeshProUGUI totalCostText;
    [SerializeField] private TextMeshProUGUI currentMoneyText;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private int structureLeftoverMoneyCount;
    [SerializeField] private List<GameObject> multiplierButtons;
    [SerializeField] private GameObject selectOnOpen;
    //list of tabs at top of menu
    [SerializeField] private List<GameObject> tabs;

    //lists in the same order as tabs, of their gradient shadows and background colours
    private List<Image> tabShadows = new();
    private List<Image> tabBackgrounds = new();
    private TextMeshProUGUI buyButtonText;
    private Image buyButtonImageComponent;
    private Image buyButtonShadow;
    private int tabIndex = 0;
    private int totalCost = 0;
    private List<GameObject> gridObjectDisplayList = new List<GameObject>();
    // A list of how many of each item there is in the players cart
    private List<int> itemCountsInCart = new List<int>();
    private int countMultiplier;
    private List<AnimationTriggers> allButtonsAnimationTrigs = new();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Multiple instances StoreManager found, deleting instance attached to {gameObject}");
            Destroy(this);
            return;
        }

        Instance = this;

        buyButtonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        buyButtonImageComponent = buyButton.GetComponent<Image>();
        buyButtonShadow = buyButton.GetComponentsInChildren<Image>()[2];

        SetUpTabImages();
    }

    private void Start()
    {
        GameManager.Instance.OnMoneyChange.AddListener(UpdateMoneyText);
        GameManager.Instance.OnMoneyChange.AddListener(UpdateMoneyColors);

        SetUpAllButtonsAnimationsList();

        countMultiplier = 1; //cannot be null
    }

    /// <summary>
    /// Toggles the state of the Store GUI (open or closed)
    /// </summary>
    /// <param name="isActive"></param>
    public void SetStoreActiveState(bool isActive)
    {
        storeGUI.SetActive(isActive);

        // When the store GUI is opened
        if (storeGUI.activeSelf)
        {
            InputDeviceManager.Instance.onGameDeviceChanged.AddListener(HandleInputDeviceType);
            OnEnableStore();
            HandleInputDeviceType(); //set first selected item if gamepad
        }
        else
        {
            InputDeviceManager.Instance.onGameDeviceChanged.RemoveListener(HandleInputDeviceType);
            totalCost = 0;
            InputSystem.actions.FindAction("Place").Enable();
        }
    }

    /// <summary>
    /// Enables all required things when the store is opened
    /// </summary>
    private void OnEnableStore()
    {
        // Fills the shopping cart with stacks of 0
        FillShoppingCartWithItems();
        SwitchTab(tabIndex);
        totalCost = CalculateTotalCost();
        totalCostText.text = $"Total: ${totalCost}";
        UpdateMoneyText();
        UpdateMoneyColors();
        buyButtonText.text = "Buy!";
        InputSystem.actions.FindAction("Place").Disable();
        ChangeMultiplierColours();
        if (TutorialManager.Instance.InProgress) TutorialManager.Instance.CompleteTutorialTask("openedShop");
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
        foreach (GameObject mult in multiplierButtons)
        {
            allButtonsAnimationTrigs.Add(mult.GetComponentInChildren<Button>().animationTriggers);
        }
        allButtonsAnimationTrigs.Add(buyButton.GetComponentInChildren<Button>().animationTriggers);
        //storeItemSlot buttons are setup in CreateGridItem()
    }

    /// <summary>
    /// Switch all button hover effects to match input device
    /// </summary>
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

    /// <summary>
    /// Changes the button, and money text visually to give feedback to user
    /// </summary>
    private void UpdateMoneyColors()
    {
        if (totalCost == 0) // no items are selected in store
        {
            SetBuyToInvalidColours();
            totalCostText.color = Color.black;
        }
        else if (totalCost > GameManager.Instance.Money) // too expensive
        {
            SetBuyToInvalidColours();
            totalCostText.color = Color.red;
        }
        else // can afford selection :D
        {
            buyButtonImageComponent.color = UIStyling.Instance.ButtonValidColor;
            buyButtonShadow.rectTransform.rotation = Quaternion.identity;
            buyButtonShadow.color = UIStyling.Instance.ShadowWhenButtonActive;
            totalCostText.color = Color.black;
        }
    }

    /// <summary>
    /// Changes the buy button to invalid colours
    /// </summary>
    private void SetBuyToInvalidColours()
    {
        buyButtonImageComponent.color = UIStyling.Instance.ButtonInvalidColor;
        buyButtonShadow.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
        buyButtonShadow.color = UIStyling.Instance.ShadowWhenButtonInvalid;
    }

    /// <summary>
    /// Sets all of the content of the store GUI
    /// </summary>
    private void SetStoreDisplayContent()
    {
        // Removes all the old GUI display gridItems, and clears the lists of what was in them
        gridObjectDisplayList.ForEach(gridItem => Destroy(gridItem));
        gridObjectDisplayList.Clear();

        int indexInTab = 0;

        // Iterates over all items to see if it should display in current tab
        for (int storeIndex = 0; storeIndex < allStoreItems.Count; storeIndex++)
        {
            // If the item is in the currently opened tab
            if (((int)allStoreItems[storeIndex].type) == tabIndex)
            {
                CreateGridItem(indexInTab, storeIndex, allStoreItems[storeIndex]);

                indexInTab++;
            }
        }
    }

    /// <summary>
    /// Instantiates a new grid item GameObject in the Store menu
    /// </summary>
    /// <param name="indexInTab"></param>
    /// <param name="storeIndex"></param>
    /// <param name="storeItem"></param>
    private void CreateGridItem(int indexInTab, int storeIndex, StoreItemSO storeItem)
    {
        // Creates the new GameObject and puts it in a list
        GameObject gridItem = Instantiate(storeItemPrefab, storeGrid.transform);
        gridObjectDisplayList.Add(gridItem);
        StoreItemSlot gridSlot = gridItem.GetComponent<StoreItemSlot>();

        // Sets all the parameters on the buttons
        gridSlot.AddButton.onClick.AddListener(() => ChangeStockCount(indexInTab, storeIndex, 1));
        gridSlot.SubtractButton.onClick.AddListener(() => ChangeStockCount(indexInTab, storeIndex, -1));
        gridSlot.NameText.text = storeItem.name;
        gridSlot.PriceText.text = $"${storeItem.cost}";
        gridSlot.CountText.text = $"{itemCountsInCart[storeIndex]}";
        gridSlot.AddButton.GetComponent<Image>().color = UIStyling.Instance.ButtonAddColor;
        gridSlot.SubtractButton.GetComponent<Image>().color = UIStyling.Instance.ButtonNegativeColor;

        //add each button to all button animation triggers list
        allButtonsAnimationTrigs.Add(gridSlot.AddButton.GetComponent<Button>().animationTriggers);
        allButtonsAnimationTrigs.Add(gridSlot.SubtractButton.GetComponent<Button>().animationTriggers);
    }

    /// <summary>
    /// Changes the stock count in your cart based on the currently selected multiplier
    /// </summary>
    /// <param name="indexInTab">The index of the changed item in its tab</param>
    /// <param name="storeIndex">The index of the changed item in the store</param>
    /// <param name="change">The amount the count is changed by</param>
    public void ChangeStockCount(int indexInTab, int storeIndex, int change)
    {
        if (countMultiplier > 1 && change > 0) //stop higher than 1 multipliers from adding more than can afford
        {
            int moneyToSpend = GameManager.Instance.Money - totalCost;
            int itemsCanAfford = moneyToSpend / allStoreItems[storeIndex].cost; //int division always rounds down
            itemsCanAfford = Mathf.Min(itemsCanAfford, countMultiplier); //dont add more than multiplier
            change = itemsCanAfford;
        }
        else if (change < 0) change *= countMultiplier; //reduce stock by multiplier

        int itemCount = itemCountsInCart[storeIndex];
        itemCount = Mathf.Max(itemCount + change, 0); //cant be less than 0
        itemCountsInCart[storeIndex] = itemCount;

        StoreItemSlot slot = gridObjectDisplayList[indexInTab].GetComponent<StoreItemSlot>();
        slot.CountText.text = itemCount.ToString();

        totalCost = CalculateTotalCost();
        totalCostText.text = $"Total: ${totalCost}";

        UpdateMoneyColors();
    }

    /// <summary>
    /// Changes the tab and resets the contents of the store
    /// </summary>
    /// <param name="index"></param>
    public void SwitchTab(int index)
    {
        tabIndex = index;
        SetStoreDisplayContent();
        ChangeTabColours();
    }

    /// <summary>
    /// Fills the shopping cart with a list of 0 count items
    /// </summary>
    private void FillShoppingCartWithItems()
    {
        itemCountsInCart.Clear();

        // Fills the cart with empty numbers
        allStoreItems.ForEach(item =>
        {
            itemCountsInCart.Add(0);
        });
    }

    /// <summary>
    /// Calculates the total cost of all selected items to purchase
    /// </summary>
    /// <returns>The total cost of current items in the cart</returns>
    private int CalculateTotalCost()
    {
        int totalCost = 0;

        for (int i = 0; i < allStoreItems.Count; i++)
        {
            totalCost += allStoreItems[i].cost * itemCountsInCart[i];
        }

        return totalCost;
    }

    /// <summary>
    /// Tries to purchase stock if the player can afford it and won't softlock themselves
    /// </summary>
    public void TryPurchaseStock()
    {
        if (GameManager.Instance.Money < totalCost)
        {
            StartCoroutine(DisplayErrorMessage("Too expensive!"));
        }
        else if (WillSoftlock())
        {
            StartCoroutine(DisplayErrorMessage("You need some stock!"));
        }
        else if (GetCartContents().Count == 0)
        {
            StartCoroutine(DisplayErrorMessage("You need to buy some stock!"));
        }
        else
        {
            PurchaseStock();
        }
    }

    /// <summary>
    /// Checks if the player will softlock themselves by making a purchase
    /// </summary>
    /// <returns>If the player will softlock themselves</returns>
    private bool WillSoftlock()
    {
        List<StoreItemSO> stockItemsInCart = new List<StoreItemSO>();
        for (int i = 0; i < allStoreItems.Count; i++)
        {
            // Count of how many of that an item there is
            if (itemCountsInCart[i] > 0 && allStoreItems[i].type == PlacementType.Stock)
            {
                stockItemsInCart.Add(allStoreItems[i]);
            }
        }

        // Checks if they have leftover money at the end
        if (GameManager.Instance.Money - totalCost >= structureLeftoverMoneyCount) return false;

        // Checks if there are any items placed in the shop
        if (StockManager.Instance.itemsToSell.Count > 0 || StockManager.Instance.customerPickedItems.Count > 0) return false;

        // Checks if there are any items in the player inventory
        List<PlaceableObject> inventoryItems = InventoryManager.Instance.InventoryPlaceableObjects.Where(placeableObject => placeableObject.type == PlacementType.Stock).ToList();
        if (inventoryItems.Count > 0) return false;

        // Checks if there is stock in cart
        if (stockItemsInCart.Count > 0) return false;

        return true;
    }

    /// <summary>
    /// Returns a list of all items inside of cart
    /// </summary>
    /// <returns></returns>
    private List<StoreItemSO> GetCartContents()
    {
        List<StoreItemSO> stockItemsInCart = new List<StoreItemSO>();
        for (int i = 0; i < allStoreItems.Count; i++)
        {
            // Count of how many of that an item there is
            if (itemCountsInCart[i] > 0)
            {
                stockItemsInCart.Add(allStoreItems[i]);
            }
        }

        return stockItemsInCart;
    }

    /// <summary>
    /// Displays too expensive text if the user cannot afford the items
    /// </summary>
    /// <param name="message"></param>
    private IEnumerator DisplayErrorMessage(string message)
    {
        buyButtonText.text = message;
        yield return new WaitForSeconds(2);
        buyButtonText.text = "Buy!";
    }

    /// <summary>
    /// Adds items to Inventory when purchasing them and removes money
    /// </summary>
    private void PurchaseStock()
    {
        GameManager.Instance.Money -= totalCost;

        // This is done via UI manager so the correct windows are opened and closed
        UIManager.Instance.SetGUIState(UIType.Store, false);

        bool hasStock = false;

        // Iterates over all items able to be bought
        for (int i = 0; i < allStoreItems.Count; i++)
        {
            // Checks if any of these are actually being bought
            if (itemCountsInCart[i] > 0)
            {
                // Gets index of item inside of inventory if it exists (if not returns -1)
                int indexOfItem = InventoryManager.Instance.InventoryPlaceableObjects.FindIndex(item => item.name == allStoreItems[i].name);

                if (indexOfItem == -1)
                {
                    StoreItemSO item = allStoreItems[i];

                    // Adds new PlaceableObject item inside of Inventory if it doesn't already exist
                    InventoryManager.Instance.InventoryPlaceableObjects.Add(new PlaceableObject(item.itemName, item.id, item, item.prefab, item.type, itemCountsInCart[i]));
                }
                else
                {
                    // Adds to the count of Inventory if the player already has that stock item
                    InventoryManager.Instance.InventoryPlaceableObjects[indexOfItem].count += itemCountsInCart[i];
                }

                if (allStoreItems[i].type == PlacementType.Stock) hasStock = true;
            }
        }

        if (TutorialManager.Instance.InProgress && hasStock)
        {
            TutorialManager.Instance.CompleteTutorialTask("boughtStock");
        }
    }

    /// <summary>
    /// Updates the money text to reflect money value in GameManager
    /// </summary>
    private void UpdateMoneyText()
    {
        currentMoneyText.text = $"${GameManager.Instance.Money}";
    }

    /// <summary>
    /// This is called when a multiplier button is clicked
    /// </summary>
    /// <param name="buttonParent">Parent of button in button prefab</param>
    public void ChangeMultiplier(GameObject buttonParent)
    {
        countMultiplier = GetIntFromButtonPrefab(buttonParent);
        ChangeMultiplierColours();
    }

    /// <summary>
    /// Gets the number displayed inside of a button's text, using the prefab that has button as a child
    /// </summary>
    /// <param name="buttonParent">The parent of the button and text objects</param>
    /// <returns>int multiplier in text</returns>
    private int GetIntFromButtonPrefab(GameObject buttonParent)
    {
        string text = "";
        int num = 0;

        // filter the num from button text
        foreach (char a in buttonParent.GetComponentInChildren<TextMeshProUGUI>().text)
        {
            if (a >= '0' && a <= '9') text += a;
        }

        // convert from text to int for math
        try
        {
            num = Convert.ToInt32(text);
        }
        catch
        {
            print(buttonParent.name + " invalid button text, must include numbers");
        }

        return num;
    }

    /// <summary>
    /// makes active button show buttonValidColor, non-active show buttonDeselectedColor
    /// </summary>
    private void ChangeMultiplierColours()
    {
        foreach (GameObject b in multiplierButtons)
        {
            Image background = b.GetComponent<Image>();
            Image shadow = b.GetComponentsInChildren<Image>()[2];

            if (GetIntFromButtonPrefab(b) == countMultiplier) //if is current multiplier
            {
                //green and dented in to show being used
                background.color = UIStyling.Instance.ButtonValidColor;
                shadow.color = UIStyling.Instance.ShadowWhenButtonActive;
                shadow.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
                
            }
            else //not current multiplier
            {
                //gray and pushed out to show unused
                background.color = UIStyling.Instance.ButtonDeselectedColor;
                shadow.color = UIStyling.Instance.ShadowWhenButtonInactive;
                shadow.rectTransform.rotation = Quaternion.identity;
            }
        }
    }

    private void SetUpTabImages()
    {
        foreach (GameObject tab in tabs)
        {
            Image[] allImages = tab.GetComponentsInChildren<Image>();
            tabBackgrounds.Add(allImages[0]); //background is parent, always first
            tabShadows.Add(allImages[allImages.Length - 1]); //shadow
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
}
