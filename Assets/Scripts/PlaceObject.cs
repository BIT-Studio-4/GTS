using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceObject : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] bool randomMode;

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
        RaycastHit hit;

        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10)) // hard coded interaction distance (10)
        {
            return;
        }

        // hard coding what objects are being placed, for now
        int objectIndex = 0;

        if (randomMode)
            objectIndex = Random.Range(0, prefabs.Count);

        GameObject placedObject = Instantiate(prefabs[objectIndex], hit.point, Quaternion.identity, placedObjects.transform);
        placedObject.transform.LookAt(transform.position);
        placedObject.transform.eulerAngles = new Vector3(
            0, placedObject.transform.eulerAngles.y, 0);
    }
}
