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

    // Handles the inventory keybind press(call this from anywhere you want to toggle the inventory)
    private void InventoryKeyPress()
    {
       SetInventoryState(InventoryManager.Instance.gameObject.activeSelf);
    }

    // Handles the store keybind press (call this from anywhere you want to toggle the store)
    private void StoreKeyPress()
    {
        SetStoreState(!StoreManager.Instance.gameObject.activeSelf);
    }

    // Method that is called to open/close the inventory(call this from anywhere you want to set the inventory)
    public void SetInventoryState(bool state)
    {
        InventoryManager.Instance.SetInventoryActiveState(state);
        StoreManager.Instance.SetStoreActiveState(false);
    }

    // Method that is called to open/close the store (call this from anywhere you want to set the store)
    public void SetStoreState(bool state)
    {
        StoreManager.Instance.SetStoreActiveState(state);
        InventoryManager.Instance.SetInventoryActiveState(false);
    }
}
