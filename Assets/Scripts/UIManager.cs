using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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
    [SerializeField] private GameObject virtualCursor;

    private bool isGUIOpen = false;
    public bool IsGUIOpen { get => isGUIOpen; }

    [SerializeField] private TextMeshProUGUI TutorialText; // Reference to tutorial text UI element

    private int currentTutorialStep = 0; // Tracks current tutorial step

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
        // force resolution so that the gamepad virtual cursor works
        // i'm sure theres a better way to get it working but this is it for now
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);

        InputSystem.actions.FindAction("ToggleInventory").performed += ctx => SetGUIState(UIType.Inventory, !InventoryManager.Instance.InventoryGUI.activeSelf);
        InputSystem.actions.FindAction("ToggleStore").performed += ctx => SetGUIState(UIType.Store, !StoreManager.Instance.StoreGUI.activeSelf);
        InputSystem.actions.FindAction("Pause").performed += ctx => SetGUIState(UIType.Pause, !pauseMenu.isActiveAndEnabled);

        InventoryManager.Instance.SetInventoryActiveState(false);
        StoreManager.Instance.SetStoreActiveState(false);
        pauseMenu.gameObject.SetActive(false);

        InputDeviceManager.Instance.onGameDeviceChanged.AddListener(ChangeCursorMode);

        StartTutorial();
    }

    private void Update()
    {
        // Check if the "N" key is pressed and advances tutorial step
        if (Keyboard.current.nKey.wasPressedThisFrame
            || Gamepad.current.dpad.right.wasPressedThisFrame)
        {
            NextTutorialStep();
        }
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
            virtualCursor.SetActive(false);
            return;
        }

        // swap between using cursor and virtual mouse
        switch (InputDeviceManager.Instance.ActiveDevice)
        {
            case InputDevice.Gamepad:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
                virtualCursor.SetActive(true);
                break;
            case InputDevice.KeyboardMouse:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                virtualCursor.SetActive(false);
                break;
        }
    }

    // Method to start the tutorial
    public void StartTutorial()
    {
        ShowTutorialStep(0); // Starts with the first step
    }

    // Method to show a specific tutorial step
    public void ShowTutorialStep(int stepIndex)
    {
        TutorialText.text = "";

        switch (stepIndex)
        {
            case 0:
                TutorialText.text = "Welcome to your very own supermarket!\n\nThis is the tutorial, which will help you to get started.\n\n(press 'N' to continue)";
                break;
            case 1:
                TutorialText.text = "We have provided you with your first shelf!\n\nPlace it somewhere in the shop by opening the inventory, and selecting the shelf from the structure tab.\n\n (Press N to continue)";
                break;
            case 2:
                TutorialText.text = "Good job!\n\nnow you have placed your first shelf, lets buy some stock to put on it.\n\nPress Q to access the shop screen, there you can purchase stock to sell for profit to customers.\n\n(Press N to continue)";
                break;
            case 3:
                TutorialText.text = "Nice!\n\nNow you can place those stock items on your shelf, by clicking on them in the inventory screen.\n\n stock items can only be placed on shelves.\n\n(Press N to continue)";
                break;
            case 4:
                TutorialText.text = "Finally, lets see if you can make profit from selling stock!\n\nThe goal is to make more than the initial money we have provided for you.\n\n(press N to end tutorial)";
                break;
        }
    }

    // Method to hide the tutorial
    public void HideTutorial()
    {
        TutorialText.text = "";
    }

    public void NextTutorialStep()
    {
        if (currentTutorialStep < 4) // Adjust based on number of steps
        {
            currentTutorialStep++;
            ShowTutorialStep(currentTutorialStep);
        }
        else
        {
            HideTutorial();
        }
    }
}
