using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the rotation of a player object. Tilts the camera for vertical rotation, spins the character controller for horizontal rotation.
/// Locks the cursor if configured to do so
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerRotation : MonoBehaviour
{
    [SerializeField][Range(45, 90)] float maxYaw;  
    [SerializeField][Range(1, 100)] float lookSensitivity;
    [SerializeField][Range(0.5f, 1)] float verticalModifier;
    [SerializeField] bool lockCursor;
    private InputAction lookAction;
    private Camera cam;
    private float yaw;

    void Awake()
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        
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

        // rotate camera vertically (around the X axis) with clamp
        yaw += -lookInput.y * verticalModifier * Time.deltaTime;
        yaw = Mathf.Clamp(yaw, -maxYaw, maxYaw);
        cam.transform.localRotation = Quaternion.Euler(yaw, 0, 0);
    }
}
