using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class InventoryTests
{
    private GameObject customerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Customer.prefab");
    private GameObject storePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/Store.prefab");
    private GameObject inventoryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/Inventory.prefab");
    private StoreItemSO itemToBuy = AssetDatabase.LoadAssetAtPath<StoreItemSO>("Assets/ScriptableObjects/StoreItems/Can.asset");

    [UnityTest]
    // make an item get sold to a customer and assert that total money afterward is correct
    public IEnumerator SellingPrices()
    {
        new GameObject().AddComponent<GameManager>();
        Customer customer = GameObject.Instantiate(customerPrefab).GetComponent<Customer>();
        SellItem item = new GameObject().AddComponent<SellItem>();
        int cost = 5;
        item.moneyOnSell = 5;
        int startMoney = GameManager.Instance.Money;
        yield return null;

        customer.TestBuyItem(item);
        yield return null;

        Assert.AreEqual(startMoney + cost, GameManager.Instance.Money);
    }

    [UnityTest]
    // make the player buy an item and assert that total money afterward is correct
    public IEnumerator SpendingPrices()
    {
        new GameObject().AddComponent<GameManager>();
        GameObject.Instantiate(inventoryPrefab);
        GameObject.Instantiate(storePrefab);
        yield return null;
        
        GameManager.Instance.Money = itemToBuy.cost;
        StoreManager.Instance.TestBuyItem(itemToBuy);
        yield return null;
        
        Assert.AreEqual(0, GameManager.Instance.Money);
    }
    
    [UnityTest]
    // make the player buy an item and assert that it is in the inventory
    public IEnumerator BuyingItemIntoInventory()
    {
        yield return null;
    }
}
