using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;

    // The list of all items that are purchasable
    [SerializeField] private List<StoreItemSO> allStoreItems = new List<StoreItemSO>();
    [SerializeField] private GameObject storeGUI;
    [SerializeField] private GameObject storeGrid;
    [SerializeField] private GameObject storeItemPrefab;
    [SerializeField] private TextMeshProUGUI totalCostText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI buyButtonText;
    private int tabIndex = 0;
    private int totalCost = 0;
    private List<GameObject> gridObjectDisplayList = new List<GameObject>();
    private List<int> purchaseItems = new List<int>();

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
        storeGUI.SetActive(false);
        InputSystem.actions.FindAction("ToggleStore").performed += ctx => ToggleStoreGUI();
        GameManager.Instance.OnMoneyChange.AddListener(UpdateMoneyText);
    }

    // This toggles the state of the Store GUI (open or closed)
    private void ToggleStoreGUI()
    {
        storeGUI.SetActive(!storeGUI.activeSelf);

        if (storeGUI.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            FillPurchaseItemList();
            SwitchTab(tabIndex);
            totalCost = CalculateTotalCost();
            totalCostText.text = $"Total: ${totalCost}";
            UpdateMoneyText();
            buyButtonText.text = "Buy!";
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            totalCost = 0;
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
        allStoreItems.ForEach(placeableObject => {
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

        allStoreItems.ForEach(item => {
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

    // Tries to purchase stock if the player can afford it
    public void TryPurchaseStock()
    {
        if (GameManager.Instance.Money >= totalCost)
        {
            PurchaseStock();
        } 
        else
        {
            StartCoroutine(DisplayTooExpensive());
        }
    }

    // Displays too expensive text if the user cannot afford the items
    private IEnumerator DisplayTooExpensive()
    {
        buyButtonText.text = "Too Expensive!";
        yield return new WaitForSeconds(2);
        buyButtonText.text = "Buy!";
    }

    // Adds items to Inventory when purchasing them and removes money
    private void PurchaseStock()
    {
        GameManager.Instance.Money -= totalCost;
        ToggleStoreGUI();

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
}
