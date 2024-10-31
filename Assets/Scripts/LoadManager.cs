using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Instance of {this} already exists, removing {this} on {gameObject}");
            Destroy(this);
            return;
        }

        Instance = this;
    }
}
