using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class HudTest
{
    private GameObject customerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Customer.prefab");

    private GameObject hudPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/HUD.prefab");

    [UnityTest]
    // make an item get sold and check that the money displayed in HUD is equal to money in inventory
    public IEnumerator HudMoneyChange()
    {
        new GameObject().AddComponent<GameManager>();
        GameObject.Instantiate(hudPrefab);
        Customer customer = GameObject.Instantiate(customerPrefab).GetComponent<Customer>();
        SellItem item = new GameObject().AddComponent<SellItem>();
        item.moneyOnSell = 5;
        yield return null;

        customer.TestBuyItem(item);
        yield return null;

        int displayedMoney = HUDManager.Instance.TestDisplayMoney();

        Assert.AreEqual(GameManager.Instance.Money, displayedMoney);
    }
}
