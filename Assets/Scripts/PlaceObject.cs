using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceObject : MonoBehaviour
{
    void Awake()
    {
        InputSystem.actions.FindAction("Place").performed += ctx => InstantiateObject();
    }

    void InstantiateObject(/*InputAction.CallbackContext ctx*/)
    {
        GameObject placedObject = new GameObject();
    }
}
