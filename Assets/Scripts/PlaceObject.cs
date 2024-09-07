using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInteraction))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();

    private GameObject placedObjects;
    private RaycastHit hit;
    private InputAction placeAction;
    private PlayerInteraction playerInteraction;
    public Vector3 Position { get; private set; }
    public Vector3 Rotation { get; private set; }

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
        if (!CanPlaceHere()) return;
        
    }

    void InstantiateObject(InputAction.CallbackContext ctx)
    {
        if (InventoryManager.Instance.HeldObject == null) return;
        if (!CanPlaceHere()) return;

        Debug.Log(InventoryManager.Instance.HeldObject);

        // places the object at the coordinates of the raycast hit
        // and becomes a child of placedObjects
        GameObject placedObject = Instantiate(InventoryManager.Instance.HeldObject.prefab, hit.point, Quaternion.identity, placedObjects.transform);
        // rotate object to opposite of player rotation
        placedObject.transform.LookAt(new Vector3(
            transform.position.x,
            placedObject.transform.position.y,
            transform.position.z));

        InventoryManager.Instance.ConsumePlacedItem();
    }

    public bool CanPlaceHere()
    {
        if (InventoryManager.Instance.HeldObject == null)
            return false;

        if (!playerInteraction.raycastHasHit)
            return false;
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
}
