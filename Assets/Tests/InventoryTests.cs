using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class InventoryTests
{
    private GameObject customerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Customer.prefab");

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
    public IEnumerator SpendingPrices()
    {
        // make the player buy an item and assert that total money afterward is correct
        yield return null;
    }
}
