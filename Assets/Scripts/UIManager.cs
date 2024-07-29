using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// The class that handles which UI is open or not at a time
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InputSystem.actions.FindAction("ToggleInventory").performed += ctx => InventoryKeyPress();
        InputSystem.actions.FindAction("ToggleStore").performed += ctx => StoreKeyPress();

        InventoryManager.Instance.SetInventoryActiveState(false);
        StoreManager.Instance.SetStoreActiveState(false);
    }

    private void InventoryKeyPress()
    {
        bool currentState = InventoryManager.Instance.gameObject.activeSelf;

        InventoryManager.Instance.SetInventoryActiveState(false);
        StoreManager.Instance.SetStoreActiveState(false);
    }

    private void StoreKeyPress()
    {
        bool currentState = StoreManager.Instance.gameObject.activeSelf;

        InventoryManager.Instance.SetInventoryActiveState(false);
        StoreManager.Instance.SetStoreActiveState(false);
    }
}
