using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class HudTest
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator HudMoneyChange()
    {
        new GameObject().AddComponent<GameManager>();
        new GameObject().AddComponent<HUDManager>();

        UIDocument TestUI = new UIDocument();
        

        // create a scenario that changes the players money and then check the HUD is displaying the correct amount 
        GameManager.Instance.Money += 5;

        //convert display into checkable amount
        string HUDdisplaytext = HUDManager.Instance.MoneyDisplay.text.Remove(0);
        int HUDmoney = int.Parse(HUDdisplaytext);

        //checking if display is correct
        Assert.AreEqual(GameManager.Instance.Money, HUDmoney);
        // Use yield to skip a frame.
        yield return null;

        
    }
}
