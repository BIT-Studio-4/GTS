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
        InputSystem.actions.FindAction("ToggleInventory").performed += ctx => InventoryKeyPress();
        InputSystem.actions.FindAction("ToggleStore").performed += ctx => StoreKeyPress();

        InventoryManager.Instance.SetInventoryActiveState(false);
        StoreManager.Instance.SetStoreActiveState(false);
    }

    // Handles the inventory keybind press(call this from anywhere you want to toggle the inventory)
    private void InventoryKeyPress()
    {
       SetInventoryState(!InventoryManager.Instance.InventoryGUI.activeSelf);
    }

    // Handles the store keybind press (call this from anywhere you want to toggle the store)
    private void StoreKeyPress()
    {
        SetStoreState(!StoreManager.Instance.StoreGUI.activeSelf);
    }

    // Method that is called to open/close the inventory(call this from anywhere you want to set the inventory)
    public void SetInventoryState(bool state)
    {
        InventoryManager.Instance.SetInventoryActiveState(state);
        StoreManager.Instance.SetStoreActiveState(false);

        SetOpenStatus(state);
    }

    // Method that is called to open/close the store (call this from anywhere you want to set the store)
    public void SetStoreState(bool state)
    {
        StoreManager.Instance.SetStoreActiveState(state);
        InventoryManager.Instance.SetInventoryActiveState(false);

        SetOpenStatus(state);
    }

    // Sets the state of if any GUI is open for the mouse and locking player e.t.c
    private void SetOpenStatus(bool state)
    {
        isGUIOpen = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
