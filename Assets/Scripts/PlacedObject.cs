using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlacedObject : MonoBehaviour
{
    private StoreItemSO storeItem;
    public StoreItemSO StoreItem;

    private void Start()
    {
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
