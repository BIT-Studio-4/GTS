using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    private StoreItemSO storeItem;
    public StoreItemSO StoreItem { get => storeItem; set => storeItem = value; }

    private void Start()
    {
        // Adds to the placed object list in StockManager
        StockManager.Instance.PlacedObjects.Add(this);
    }

    /// <summary>
    /// When destroying a placed object do it through this method, it is used to keep track of whats placed and whats not
    /// </summary>
    public void DestroyPlacedObject()
    {
        StockManager.Instance.PlacedObjects.Remove(this);
        Destroy(gameObject);
    }
}
