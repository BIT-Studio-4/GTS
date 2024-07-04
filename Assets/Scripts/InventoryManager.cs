using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private List<PlaceableObject> placeableObjects = new List<PlaceableObject>();
    [SerializeField] private GameObject inventoryGUI;
    [SerializeField] private GameObject inventoryGrid;
    [SerializeField] private GameObject inventoryItemPrefab;

    private PlaceableObject heldObject;
    public PlaceableObject HeldObject { get { return heldObject; } set { heldObject = value; } }
    private int tabIndex;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        inventoryGUI.SetActive(false);
        HeldObject = null;
        InputSystem.actions.FindAction("ToggleInventory").performed += ctx => ToggleInventoryGUI();
        SwitchTab(0);
    }
    
    private void ToggleInventoryGUI()
    {
        inventoryGUI.SetActive(!inventoryGUI.activeSelf);

        Cursor.lockState = inventoryGUI.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        SwitchTab(tabIndex);
    }

    public void SwitchTab(int index)
    {
        tabIndex = index;
        Debug.Log($"New Tab Index is {index}");
        SetInventoryContent();
    }

    private void SetInventoryContent()
    {
        
    }

    public void StockButtonClick(int index)
    {
        Debug.Log($"New stock Index is {index}");
    }
}
