using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using static UnityEditor.Progress;

public class StoreManager : MonoBehaviour
{
    private struct PurchaseItem
    {
        public int count;
    }

    public static StoreManager Instance;

    // The list of all items that are purchasable
    [SerializeField] private List<StoreItemSO> allStoreItems = new List<StoreItemSO>();
    [SerializeField] private GameObject storeGUI;
    [SerializeField] private GameObject storeGrid;
    [SerializeField] private GameObject storeItemPrefab;
    [SerializeField] private TextMeshProUGUI totalCostText;
    private int tabIndex = 0;
    private List<GameObject> gridObjectDisplayList = new List<GameObject>();
    private List<PurchaseItem> purchaseItems = new List<PurchaseItem>();

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
            totalCostText.text = $"Total: ${CalculateTotalCost()}";
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
        gridSlot.CountText.text = $"{purchaseItems[storeIndex].count}";
    }

    // Runs when the amount of stock the player wants is added to or subtracted from
    public void ChangeStockCount(int UIIndex, int storeIndex, int change)
    {
        PurchaseItem countChange = purchaseItems[storeIndex];
        countChange.count = Mathf.Max(countChange.count + change, 0);
        purchaseItems[storeIndex] = countChange;

        StoreItemSlot slot = gridObjectDisplayList[UIIndex].GetComponent<StoreItemSlot>();
        slot.CountText.text = countChange.count.ToString();

        totalCostText.text = $"Total: ${CalculateTotalCost()}";
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
            PurchaseItem purchaseItem = new PurchaseItem();
            purchaseItem.count = 0;
            purchaseItems.Add(purchaseItem);
            Debug.Log(item.itemName);
        });
    }

    // Calculates the total cost of all selected items to purchase
    private int CalculateTotalCost()
    {
        int totalCost = 0;

        for (int i = 0; i < allStoreItems.Count; i++)
        {
            totalCost += allStoreItems[i].cost * purchaseItems[i].count;
        }

        return totalCost;
    }
}
