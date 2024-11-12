using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;

// Which inventory do you want to apply things to
public enum UIType
{
    Inventory,
    Store,
    Pause
}

/// <summary>
/// The class that handles which UI is open or not at a time
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private EventSystem eventSystemMain;
    public EventSystem EventSystemMain { get => eventSystemMain; }

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
        InputSystem.actions.FindAction("Pause").performed += ctx => SetGUIState(UIType.Pause, !pauseMenu.isActiveAndEnabled);

        InventoryManager.Instance.SetInventoryActiveState(false);
        StoreManager.Instance.SetStoreActiveState(false);
        pauseMenu.gameObject.SetActive(false);

        InputDeviceManager.Instance.onGameDeviceChanged.AddListener(ChangeCursorMode);
    }


    /// <summary>
    /// Used to open/close a specific GUI
    /// </summary>
    /// <param name="UI"></param>
    /// <param name="state"></param>
    public void SetGUIState(UIType UI, bool state)
    {
        switch (UI)
        {
            case UIType.Inventory:
                if (pauseMenu.isActiveAndEnabled) return;
                InventoryManager.Instance.SetInventoryActiveState(state);
                StoreManager.Instance.SetStoreActiveState(false);
                break;
            case UIType.Store:
                if (pauseMenu.isActiveAndEnabled) return;
                StoreManager.Instance.SetStoreActiveState(state);
                InventoryManager.Instance.SetInventoryActiveState(false);
                break;
            case UIType.Pause:
                InventoryManager.Instance.SetInventoryActiveState(false);
                StoreManager.Instance.SetStoreActiveState(false);
                // close gui (but not pause) if gui open
                if (isGUIOpen && !pauseMenu.isActiveAndEnabled)
                {
                    SetOpenStatus(false);
                    return;
                }
                pauseMenu.gameObject.SetActive(state);
                break;
        }

        SetOpenStatus(state);
    }

    // Sets the state of if any GUI is open for the mouse and locking player e.t.c
    private void SetOpenStatus(bool state)
    {
        isGUIOpen = state;
        ChangeCursorMode();
    }

    private void ChangeCursorMode()
    {
        // not much logic to take care of when GUI is closed, so get that out the way
        if (!isGUIOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }

        // swap between using cursor and UI navigation
        switch (InputDeviceManager.Instance.ActiveDevice)
        {
            case InputDevice.Gamepad:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
                break;
            case InputDevice.KeyboardMouse:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }
}
