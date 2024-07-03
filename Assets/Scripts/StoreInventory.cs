using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreInventory : MonoBehaviour
{
    public static StoreInventory Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
