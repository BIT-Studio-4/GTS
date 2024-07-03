using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private List<PlaceableObject> stockObjects = new List<PlaceableObject>();
    [SerializeField] private List<PlaceableObject> structureObjects = new List<PlaceableObject>();
    [SerializeField] private GameObject inventoryGUI;

    private PlaceableObject heldObject;
    public PlaceableObject HeldObject { get { return heldObject; } set { heldObject = value; } }

    private InputAction toggleInventoryAction;

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
        toggleInventoryAction = InputSystem.actions.FindAction("ToggleInventory");
    }

    void Update()
    {
        if (toggleInventoryAction.WasPressedThisFrame())
        {
            ToggleInventoryGUI();

        }
    }

    private void ToggleInventoryGUI()
    {
        inventoryGUI.SetActive(!inventoryGUI.activeSelf);
        Debug.Log("Toggled Inventory GUI");
    }
}
