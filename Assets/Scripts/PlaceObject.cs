using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInteraction))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();

    private Vector3 position;
    public Vector3 Position { get => position; }
    public Quaternion Rotation { get; private set; }
    public bool CanPlaceHere { get; private set; }

    private GameObject placedObjects;
    private RaycastHit hit;
    private InputAction placeAction;
    private PlayerInteraction playerInteraction;
    private Grid grid;

    void Awake()
    {
        // empty parent to keep all instantiated objects hidden in hierarchy
        placedObjects = new GameObject("Placed Objects");
        // add listener to place input action
        placeAction = InputSystem.actions.FindAction("Place");
        placeAction.performed += ctx => InstantiateObject(ctx);
        playerInteraction = GetComponent<PlayerInteraction>();
    }

    private void Update()
    {
        if (InventoryManager.Instance.HeldObject == null) return;
        
        position = hit.point;
        SetRotationRelativeToPlayer();
                    
        if (InventoryManager.Instance.HeldObject.type == PlacementType.Structure)
        {
            SnapPosition();
            SnapRotation();
        }

        CanPlaceHere = IsPlacementValid();
    }

    void InstantiateObject(InputAction.CallbackContext ctx)
    {
        if (InventoryManager.Instance.HeldObject == null) return;
        if (!IsPlacementValid()) return;

        Debug.Log(InventoryManager.Instance.HeldObject);

        // places the object at the coordinates of the raycast hit
        // and becomes a child of placedObjects
        GameObject placedObject = Instantiate(InventoryManager.Instance.HeldObject.prefab, Position, Rotation, placedObjects.transform);

        InventoryManager.Instance.ConsumePlacedItem();
    }

    public bool IsPlacementValid()
    {
        if (!playerInteraction.raycastHasHit) return false;
        hit = playerInteraction.Hit;

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

        return true;
    }

    void SnapPosition()
    {
        if (!hit.transform.parent.TryGetComponent<Grid>(out grid)) return;
        position = grid.GetCellCenterWorld(grid.WorldToCell(hit.point));
        position.y = hit.point.y;
    }
    
    void SnapRotation()
    {
        Vector3 eulers = Rotation.eulerAngles;
        eulers.y = Mathf.Round(eulers.y / 90) * 90;
        Rotation = Quaternion.Euler(eulers);
    }
    
    void SetRotationRelativeToPlayer()
    {
        Vector3 directionToPlayer = -transform.forward;
        directionToPlayer.y = 0;
        Rotation = Quaternion.LookRotation(-directionToPlayer);
    }
}
