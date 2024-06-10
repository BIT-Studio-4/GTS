using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class PlayerRotation : MonoBehaviour
{
    [SerializeField] float maxYaw;  
    [SerializeField][Range(1, 100)] float lookSensitivity;
    [SerializeField][Range(0.5f, 1)] float verticalModifier;
    private InputAction lookAction;
    private CharacterController cc;
    private Camera cam;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        cc = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        lookAction = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>() * lookSensitivity;

        // rotate character horizontally (around the Y axis)
        transform.Rotate(Vector3.up * lookInput.x * Time.deltaTime);

        // rotate camera vertically (around the X axis)
        cam.transform.Rotate(Vector3.right * -lookInput.y * Time.deltaTime);
    }
}
