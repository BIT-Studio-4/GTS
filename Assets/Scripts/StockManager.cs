using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour
{
    public static StockManager Instance;



    private void Awake()
    { 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // Changes the tab and resets the contents of the inventory
    public void SwitchTab(int index)
    {
        //tabIndex = index;
        //SetInventoryDisplayContent();
    }
}
