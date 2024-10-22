using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HudTest
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator HudMoneyChange()
    {

        // create a scenario that changes the players money and then check the HUD is displaying the correct amount 
        
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;

        
    }
}
