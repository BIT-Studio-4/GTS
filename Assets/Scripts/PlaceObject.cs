using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles placement of held object with grid snapping.
/// Uses GhostObjectPlacement to check for object intersection
/// </summary>
[RequireComponent(typeof(PlayerInteraction))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] private GhostObjectPlacement ghostObject;

    private Vector3 position;
    private Quaternion rotation;
    public bool CanPlaceHere { get; private set; }

    private GameObject placedObjects;
    private RaycastHit hit;
    private InputAction placeAction;
    private InputAction disableGridAction;
    private InputAction rotateObjectAction;
    private PlayerInteraction playerInteraction;
    private Grid grid;
    private int rotationSnapDegrees;

    void Awake()
    {
        // empty parent to keep all instantiated objects hidden in hierarchy
        placedObjects = new GameObject("Placed Objects");
        // add listener to place input action
        placeAction = InputSystem.actions.FindAction("Place");
        placeAction.performed += ctx => InstantiateObject();
        rotateObjectAction = InputSystem.actions.FindAction("RotateObject");
        rotateObjectAction.performed += ctx => RotateObject();
        disableGridAction = InputSystem.actions.FindAction("DisableGridSnapping");

        playerInteraction = GetComponent<PlayerInteraction>();
    }

    void Start()
    {
        InventoryManager.Instance.OnHeldObjectChange += HandleHeldObjectChange;
    }

    void Update()
    {
        if (InventoryManager.Instance.HeldObject == null || !playerInteraction.raycastHasHit)
        {
            CanPlaceHere = false;
            ghostObject.gameObject.SetActive(false);
            return;
        }
        hit = playerInteraction.Hit;
        ghostObject.gameObject.SetActive(true);

        position = hit.point;
        SnapPosition();
        SnapRotation();
        ghostObject.UpdateTransform(position, rotation);

        CanPlaceHere = IsPlacementValid();
        ghostObject.canBePlaced = CanPlaceHere;
    }

    /// <summary>
    /// Places held object at position and rotation
    /// </summary>
    void InstantiateObject()
    {
        if (InventoryManager.Instance.HeldObject == null) return;
        if (!IsPlacementValid()) return;

        // places the object at the coordinates of the raycast hit
        // and becomes a child of placedObjects
        GameObject placedObject = Instantiate(InventoryManager.Instance.HeldObject.prefab, position, rotation, placedObjects.transform);

        InventoryManager.Instance.ConsumePlacedItem();
    }

    /// <summary>
    /// All-in-one checklist to determine if placing
    /// object should be successful
    /// </summary>
    /// <returns>True if object can be placed</returns>
    public bool IsPlacementValid()
    {
        if (Vector3.Angle(hit.normal, Vector3.up) > 5f) //angle threshhold to place objects on flat surfaces only
        {
            if (placeAction.WasPressedThisFrame())
                HUDManager.Instance.ErrorPopup("Can't place object on non-flat surface");
            return false;
        }

        // If the held object is a Stock item, restrict placement to shelves only
        if (InventoryManager.Instance.HeldObject.type == PlacementType.Stock)
        {
            if (!hit.collider.CompareTag("Shelf"))
            {
                if (placeAction.WasPressedThisFrame())
                    HUDManager.Instance.ErrorPopup("Stock items can only be placed on shelves");
                return false;
            }
        }

        // If the held object is a Shelf item, restrict placement to "Floor" tag only
        if (InventoryManager.Instance.HeldObject.type == PlacementType.Structure)
        {
            if (!hit.collider.CompareTag("Floor"))
            {
                if (placeAction.WasPressedThisFrame())
                    HUDManager.Instance.ErrorPopup("Shelves can only be placed on the floor");
                return false;
            }
        }

        // If there are any intersecting objects
        if (ghostObject.isIntersecting)
        {
            if (placeAction.WasPressedThisFrame())
            {
                HUDManager.Instance.ErrorPopup("Items cannot intersect");
            }
            return false;
        }

        return true;
    }

    void SnapPosition()
    {
        if (disableGridAction.inProgress) return;
        // only shelves on the floor should snap to grid
        if (InventoryManager.Instance.HeldObject.type == PlacementType.Structure
            && !hit.collider.CompareTag("Floor")) return;
        grid = hit.transform.GetComponentInParent<Grid>();
        if (grid == null) return;
        position = grid.GetCellCenterWorld(grid.WorldToCell(hit.point));
        position.y = hit.point.y;
    }

    void SnapRotation()
    {
        Vector3 eulers = rotation.eulerAngles;
        eulers.y = Mathf.Round(eulers.y / rotationSnapDegrees) * rotationSnapDegrees;
        rotation = Quaternion.Euler(eulers);
    }

    void SetRotationRelativeToPlayer()
    {
        Vector3 directionToPlayer = -transform.forward;
        directionToPlayer.y = 0;
        rotation = Quaternion.LookRotation(directionToPlayer);
    }

    void HandleHeldObjectChange(PlaceableObject x)
    {
        if (InventoryManager.Instance.HeldObject == null) return;
        ghostObject.GetMeshFromHeldObject();
        SetRotationRelativeToPlayer();
        rotationSnapDegrees = GetRotationSnapDegrees();
    }

    /// <summary>
    /// Determines how many degrees to snap rotation by
    /// based on type of object
    /// </summary>
    /// <returns>rotationSnapDegrees</returns>
    int GetRotationSnapDegrees()
    {
        switch (InventoryManager.Instance.HeldObject.type)
        {
            case PlacementType.Structure:
                return 90;
            default:
                return 15;
        }
    }

    /// <summary>
    /// Rotates object by number of degrees, specified by rotationSnapDegrees
    /// </summary>
    void RotateObject()
    {
        Vector3 eulers = rotation.eulerAngles;
        float direction = rotateObjectAction.ReadValue<float>();
        Debug.Log(direction);
        eulers += Vector3.up * rotationSnapDegrees * direction;
        rotation = Quaternion.Euler(eulers);
    }
}
