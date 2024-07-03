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
        placedObjects = new GameObject("Placed Objects"); // empty parent to keep all instantiated objects hidden in hierarchy
        InputSystem.actions.FindAction("Place").performed += ctx => InstantiateObject(ctx);
    }

    void InstantiateObject(InputAction.CallbackContext ctx)
    {
        RaycastHit hit;

        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10)) // hard coded interaction distance (10)
        {
            return;
        }

        GameObject placedObject = Instantiate(prefabs[0], hit.point, Quaternion.identity, placedObjects.transform);
    }
}
