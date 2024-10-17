using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SellItemEvent : UnityEvent<SellItem> { }

public class StockManager : MonoBehaviour
{
    public static StockManager Instance;

    [HideInInspector] public List<SellItem> itemsToSell;
    [HideInInspector] public List<SellItem> customerPickedItems;

    [HideInInspector] public SellItemEvent itemPlaced = new SellItemEvent();
    [HideInInspector] public SellItemEvent itemPickedByCustomer = new SellItemEvent();
    [HideInInspector] public SellItemEvent itemSold = new SellItemEvent();

    // A list of all objects that are placed down in the store
    [SerializeField] private List<PlacedObject> placedObjects = new List<PlacedObject>();
    public List<PlacedObject> PlacedObjects { get => placedObjects; set => placedObjects = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Already another instance of StockManager!!");
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        itemPlaced.AddListener(OnItemPlaced);
        itemPickedByCustomer.AddListener(OnItemPicked);
        itemSold.AddListener(OnItemSold);
    }

    /// <summary>
    /// This is called when an item is placed
    /// </summary>
    /// <param name="item"></param>
    private void OnItemPlaced(SellItem item)
    {
        itemsToSell.Add(item);
    }

    /// <summary>
    /// This is called when a customer picks an item to purchase
    /// </summary>
    /// <param name="item"></param>
    private void OnItemPicked(SellItem item)
    {
        itemsToSell.Remove(item);
        customerPickedItems.Add(item);
    }

    /// <summary>
    /// This is called when a customer has purchased an item
    /// </summary>
    /// <param name="item"></param>
    private void OnItemSold(SellItem item)
    {
        customerPickedItems.Remove(item);

        // Special case because we don't want to track the item after its been picked up
        PlacedObject placedObject = item.gameObject.GetComponent<PlacedObject>();
        placedObjects.Remove(placedObject);
        // Removing the component on the object, not strictly necessary but could cause some issues if it remains
        Destroy(placedObject);
    }

    public SellItem GetRandomItemForSale()
    {
        return itemsToSell[Random.Range(0, itemsToSell.Count)];
    }
}
