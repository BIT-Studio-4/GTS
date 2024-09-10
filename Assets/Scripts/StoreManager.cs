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
    [SerializeField] private GameObject storeGrid;
    [SerializeField] private GameObject storeItemPrefab;
    [SerializeField] private TextMeshProUGUI totalCostText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private Color buttonValidColor;
    [SerializeField] private Color buttonInvalidColor;
    [SerializeField] private int structureLeftoverMoneyCount;
    [SerializeField] private List<Button> multiplierButtons;
    private TextMeshProUGUI buyButtonText;
    private Image buyButtonImage;
    private int tabIndex = 0;
    private int totalCost = 0;
    private List<GameObject> gridObjectDisplayList = new List<GameObject>();
    private List<int> purchaseItems = new List<int>();
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
        buyButtonImage = buyButton.GetComponent<Image>();
    }

    private void Start()
    {
        GameManager.Instance.OnMoneyChange.AddListener(UpdateMoneyText);
        GameManager.Instance.OnMoneyChange.AddListener(UpdateMoneyColors);

        countMultiplier = 1; //cannot be null
    }

    // This toggles the state of the Store GUI (open or closed)
    public void SetStoreActiveState(bool isActive)
    {
        storeGUI.SetActive(isActive);

        if (storeGUI.activeSelf)
        {
            FillPurchaseItemList();
            SwitchTab(tabIndex);
            totalCost = CalculateTotalCost();
            totalCostText.text = $"Total: ${totalCost}";
            UpdateMoneyText();
            UpdateMoneyColors();
            buyButtonText.text = "Buy!";
            InputSystem.actions.FindAction("Place").Disable();
        }
        else
        {
            totalCost = 0;
            InputSystem.actions.FindAction("Place").Enable();
        }
    }

    private void UpdateMoneyColors()
    {
        if (totalCost == 0) // no items are selected in store
        {
            buyButtonImage.color = buttonInvalidColor;
            totalCostText.color = Color.black;
        }
        else if (totalCost > GameManager.Instance.Money) // too expensive
        {
            buyButtonImage.color = buttonInvalidColor;
            totalCostText.color = Color.red;
        }
        else // can afford selection :D
        {
            buyButtonImage.color = buttonValidColor;
            totalCostText.color = Color.black;
        }
    }

    // Sets all of the content of the store GUI
    private void SetStoreDisplayContent()
    {
        // Removes all the old GUI display gridItems, and clears the lists of what was in them
        gridObjectDisplayList.ForEach(gridItem => Destroy(gridItem));
        gridObjectDisplayList.Clear();

        int UIIndex = 0;
        int storeIndex = 0;

        // Iterates over all items to see if it should display in current tab
        allStoreItems.ForEach(placeableObject =>
        {
            if (((int)placeableObject.type) == tabIndex)
            {
                CreateGridItem(UIIndex, storeIndex, placeableObject);

                UIIndex++;
            }

            storeIndex++;
        });
    }

    // Instantiates a new grid GameObject in the Store menu
    private void CreateGridItem(int UIIndex, int storeIndex, StoreItemSO storeItem)
    {
        // Creates the new GameObject and puts it in a list
        GameObject gridItem = Instantiate(storeItemPrefab, storeGrid.transform);
        gridObjectDisplayList.Add(gridItem);
        StoreItemSlot gridSlot = gridItem.GetComponent<StoreItemSlot>();

        gridSlot.AddButton.onClick.AddListener(() => ChangeStockCount(UIIndex, storeIndex, 1));
        gridSlot.SubtractButton.onClick.AddListener(() => ChangeStockCount(UIIndex, storeIndex, -1));
        gridSlot.NameText.text = storeItem.name;
        gridSlot.PriceText.text = $"${storeItem.cost}";
        gridSlot.CountText.text = $"{purchaseItems[storeIndex]}";
    }

    // Runs when the amount of stock the player wants is added to or subtracted from
    public void ChangeStockCount(int UIIndex, int storeIndex, int change)
    {
        int countChange = purchaseItems[storeIndex];
        countChange = Mathf.Max(countChange + change, 0);
        purchaseItems[storeIndex] = countChange;

        StoreItemSlot slot = gridObjectDisplayList[UIIndex].GetComponent<StoreItemSlot>();
        slot.CountText.text = countChange.ToString();

        totalCost = CalculateTotalCost();
        totalCostText.text = $"Total: ${totalCost}";

        UpdateMoneyColors();
    }

    // Changes the tab and resets the contents of the store
    public void SwitchTab(int index)
    {
        tabIndex = index;
        SetStoreDisplayContent();
    }

    // Fills the list of all empty items possible to purchase
    private void FillPurchaseItemList()
    {
        purchaseItems.Clear();

        allStoreItems.ForEach(item =>
        {
            purchaseItems.Add(0);
        });
    }

    // Calculates the total cost of all selected items to purchase
    private int CalculateTotalCost()
    {
        int totalCost = 0;

        for (int i = 0; i < allStoreItems.Count; i++)
        {
            totalCost += allStoreItems[i].cost * purchaseItems[i];
        }

        return totalCost;
    }

    // Tries to purchase stock if the player can afford it and it won't softlock themselves
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
            if (purchaseItems[i] > 0 && allStoreItems[i].type == PlacementType.Stock)
            {
                stockItemsInCart.Add(allStoreItems[i]);
            }
        }
        
        // Checks if there is stock in cart
        if (stockItemsInCart.Count > 0) return false;

        return true;
    }

    // Displays too expensive text if the user cannot afford the items
    private IEnumerator DisplayErrorMessage(string message)
    {
        buyButtonText.text = message;
        yield return new WaitForSeconds(2);
        buyButtonText.text = "Buy!";
    }

    // Adds items to Inventory when purchasing them and removes money
    private void PurchaseStock()
    {
        GameManager.Instance.Money -= totalCost;

        // This is done via UI manager so the correct windows are opened and closed
        UIManager.Instance.SetGUIState(UIType.Store, false);

        // Iterates over all items able to be bought
        for (int i = 0; i < allStoreItems.Count; i++)
        {
            // Checks if any of these are actually being bought
            if (purchaseItems[i] > 0)
            {
                // Gets index of item inside of inventory if it exists (if not returns -1)
                int indexOfItem = InventoryManager.Instance.InventoryPlaceableObjects.FindIndex(item => item.name == allStoreItems[i].name);

                if (indexOfItem == -1)
                {
                    StoreItemSO item = allStoreItems[i];

                    // Adds new PlaceableObject item inside of Inventory if it doesn't already exist
                    InventoryManager.Instance.InventoryPlaceableObjects.Add(new PlaceableObject(item.itemName, item.prefab, item.type, purchaseItems[i]));
                }
                else
                {
                    // Adds to the count of Inventory if the player already has that stock item
                    InventoryManager.Instance.InventoryPlaceableObjects[indexOfItem].count += purchaseItems[i];
                }
            }
        }
    }

    // Updates the money text to reflect money value in GameManager
    private void UpdateMoneyText()
    {
        moneyText.text = $"${GameManager.Instance.Money}";
    }

    /// <summary>
    /// This is called when a multiplier button is clicked, 
    /// It makes countMulitplier the numbers from button clicked
    /// </summary>
    /// <param name="button"></param>
    public void ChangeMultiplier(Button button)
    {
        // filter the numbers from button text
        string multiText = "";
        foreach (char a in button.GetComponentInChildren<TextMeshProUGUI>().text)
        {
            if (a >= '0' && a <= '9') multiText += a;
        }

        // convert from text to int for math
        try
        {
            countMultiplier = Convert.ToInt32(multiText);
            print(countMultiplier + " successfully changed"); //--------REMOVE LATER-------\\
        }
        catch
        {
            print(button.name + " invalid mulitiplier text, must include numbers");
        }

        // make active button show buttonValidColor, non-active show buttonInvalidColor
        button.GetComponent<Image>().color = buttonValidColor;
        foreach (Button b in multiplierButtons)
        {
            if (b != button) b.GetComponent<Image>().color = buttonInvalidColor;
        }
    }
}
