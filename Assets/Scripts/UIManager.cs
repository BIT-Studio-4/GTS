using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Which inventory do you want to apply things to
public enum UIType
{
    Inventory,
    Store
}

/// <summary>
/// The class that handles which UI is open or not at a time
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private bool isGUIOpen = false;
    public bool IsGUIOpen { get => isGUIOpen; }

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
        InputSystem.actions.FindAction("ToggleInventory").performed += ctx => SetGUIState(UIType.Inventory, !InventoryManager.Instance.InventoryGUI.activeSelf);
        InputSystem.actions.FindAction("ToggleStore").performed += ctx => SetGUIState(UIType.Store, !StoreManager.Instance.StoreGUI.activeSelf);

        InventoryManager.Instance.SetInventoryActiveState(false);
        StoreManager.Instance.SetStoreActiveState(false);
    }

    // Method that is called to open/close a GUI (call this from anywhere you want to set the gui)
    public void SetGUIState(UIType UI, bool state)
    {
        switch (UI)
        {
            case UIType.Inventory:
                InventoryManager.Instance.SetInventoryActiveState(state);
                StoreManager.Instance.SetStoreActiveState(false);
                break;
            case UIType.Store:
                StoreManager.Instance.SetStoreActiveState(state);
                InventoryManager.Instance.SetInventoryActiveState(false);
                break;
        }

        SetOpenStatus(state);
    }

    // Sets the state of if any GUI is open for the mouse and locking player e.t.c
    private void SetOpenStatus(bool state)
    {
        isGUIOpen = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
