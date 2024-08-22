using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInteraction))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();

    private GameObject placedObjects;
    public UnityEvent<string> incorrectPlacement = new UnityEvent<string>();    private PlayerInteraction playerInteraction;
    private RaycastHit hit;
    private InputAction placeAction;

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

        if (!playerInteraction.raycastHasHit) return;
        hit = playerInteraction.Hit;

        if (Vector3.Angle(hit.normal, Vector3.up) > 5f) //angle threshhold to place objects on flat surfaces only
        {
            if (placeAction.WasPressedThisFrame())
                Debug.Log("Can't place object on non-flat surface");
            InventoryManager.Instance.HeldObject.canBePlacedAtHit = false;
            return;
        }

        // If the held object is a Stock item, restrict placement to shelves only
        if (InventoryManager.Instance.HeldObject.type == PlacementType.Stock)
        {
            if (!hit.collider.CompareTag("Shelf"))
            {
                if (placeAction.WasPressedThisFrame())
                    incorrectPlacement.Invoke("Stock items can only be placed on shelves");
                InventoryManager.Instance.HeldObject.canBePlacedAtHit = false;
                return;
            }
        }

        InventoryManager.Instance.HeldObject.canBePlacedAtHit = true;
    }

    void InstantiateObject(InputAction.CallbackContext ctx)
    {
        if (InventoryManager.Instance.HeldObject == null) return;
        if (!InventoryManager.Instance.HeldObject.canBePlacedAtHit) return;

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
}
