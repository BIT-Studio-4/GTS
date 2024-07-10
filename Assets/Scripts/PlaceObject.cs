using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceObject : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();

    private GameObject placedObjects;

    void Awake()
    {
        // empty parent to keep all instantiated objects hidden in hierarchy
        placedObjects = new GameObject("Placed Objects");
        // add listener to place input action
        InputSystem.actions.FindAction("Place").performed += ctx => InstantiateObject(ctx);
    }

    void InstantiateObject(InputAction.CallbackContext ctx)
    {
        Debug.Log(InventoryManager.Instance.HeldObject);

        if (InventoryManager.Instance.HeldObject == null)
            return;

        RaycastHit hit;

        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10)) // hard coded interaction distance (10)
            return;

        // places the object at the coordinates of the raycast hit
        // and becomes a child of placedObjects
        GameObject placedObject = Instantiate(InventoryManager.Instance.HeldObject.prefab, hit.point, Quaternion.identity, placedObjects.transform);
        // rotate object to opposite of player rotation
        placedObject.transform.LookAt(new Vector3(
            transform.position.x,
            placedObject.transform.position.y,
            transform.position.z));
    }
}
