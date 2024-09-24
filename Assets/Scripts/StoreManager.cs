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
    [SerializeField] private GameObject storeGUI;
    public GameObject StoreGUI {  get => storeGUI; set => storeGUI = value; }
    // The grid that aligns the buttons in the UI
    [SerializeField] private GameObject storeGrid;
    // The prefab for each item displayed in the UI
    [SerializeField] private GameObject storeItemPrefab;
    [SerializeField] private TextMeshProUGUI totalCostText;
    [SerializeField] private TextMeshProUGUI currentMoneyText;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private Color buttonValidColor;
    [SerializeField] private Color buttonInvalidColor;
    [SerializeField] private int structureLeftoverMoneyCount;
    [SerializeField] private List<Button> multiplierButtons;
    private TextMeshProUGUI buyButtonText;
    private Image buyButtonImageComponent;
    private int tabIndex = 0;
    private int totalCost = 0;
    private List<GameObject> gridObjectDisplayList = new List<GameObject>();
    // A list of how many of each item there is in the players cart
    private List<int> itemCountsInCart = new List<int>();
    private int countMultiplier;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        buyButtonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        buyButtonImageComponent = buyButton.GetComponent<Image>();
    }

    private void Start()
    {
        GameManager.Instance.OnMoneyChange.AddListener(UpdateMoneyText);
        GameManager.Instance.OnMoneyChange.AddListener(UpdateMoneyColors);

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
            OnEnableStore();
        }
        else
        {
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
    }

    /// <summary>
    /// Changes the button, and money text visually to give feedback to user
    /// </summary>
    private void UpdateMoneyColors()
    {
        if (totalCost == 0) // no items are selected in store
        {
            buyButtonImageComponent.color = buttonInvalidColor;
            totalCostText.color = Color.black;
        }
        else if (totalCost > GameManager.Instance.Money) // too expensive
        {
            buyButtonImageComponent.color = buttonInvalidColor;
            totalCostText.color = Color.red;
        }
        else // can afford selection :D
        {
            buyButtonImageComponent.color = buttonValidColor;
            totalCostText.color = Color.black;
        }
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
        // Checks if they have leftover money at the end
        if (GameManager.Instance.Money - totalCost >= structureLeftoverMoneyCount) return false;

        // Checks if there are any items placed in the shop
        if (StockManager.Instance.itemsToSell.Count > 0 || StockManager.Instance.customerPickedItems.Count > 0) return false;

        // Checks if there are any items in the player inventory
        List<PlaceableObject> inventoryItems = InventoryManager.Instance.InventoryPlaceableObjects.Where(placeableObject => placeableObject.type == PlacementType.Stock).ToList();
        if (inventoryItems.Count > 0) return false;

        // Makes a list of all stock items inside of cart
        List<StoreItemSO> stockItemsInCart = new List<StoreItemSO>();
        for (int i = 0; i < allStoreItems.Count; i++)
        {
            // Count of how many of that an item there is
            if (itemCountsInCart[i] > 0 && allStoreItems[i].type == PlacementType.Stock)
            {
                stockItemsInCart.Add(allStoreItems[i]);
            }
        }
        
        // Checks if there is stock in cart
        if (stockItemsInCart.Count > 0) return false;

        return true;
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
                    InventoryManager.Instance.InventoryPlaceableObjects.Add(new PlaceableObject(item.itemName, item.prefab, item.type, itemCountsInCart[i]));
                }
                else
                {
                    // Adds to the count of Inventory if the player already has that stock item
                    InventoryManager.Instance.InventoryPlaceableObjects[indexOfItem].count += itemCountsInCart[i];
                }
            }
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
    /// <param name="button"></param>
    public void ChangeMultiplier(Button button)
    {
        countMultiplier = GetIntFromButton(button);
        ChangeMultiplierColours();
    }

    /// <summary>
    /// Gets the number displayed inside of a button's text
    /// </summary>
    /// <param name="button"></param>
    /// <returns>int</returns>
    private int GetIntFromButton(Button button)
    {
        string text = "";
        int num = 0;

        // filter the num from button text
        foreach (char a in button.GetComponentInChildren<TextMeshProUGUI>().text)
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
            print(button.name + " invalid button text, must include numbers");
        }

        return num;
    }

    /// <summary>
    /// makes active button show buttonValidColor, non-active show buttonInvalidColor
    /// </summary>
    private void ChangeMultiplierColours()
    {
        foreach (Button b in multiplierButtons)
        {
            if (GetIntFromButton(b) == countMultiplier) b.GetComponent<Image>().color = buttonValidColor;
            else b.GetComponent<Image>().color = buttonInvalidColor;
        }
    }
}
