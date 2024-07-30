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
    [SerializeField][Range(0.01f, 1)] float lookSensitivity;
    [SerializeField][Range(0.5f, 1)] float verticalModifier;
    [SerializeField] bool lockCursor;
    private InputAction lookAction;
    private bool inputIsDelta;
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
        // delta is mouse or touch control
        // this will set inputIsDelta whenever the player looks around
        lookAction.performed += ctx => { inputIsDelta = ctx.control.name == "delta"; };
    }

    void Update()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>() * lookSensitivity;
        // only mutliply by deltaTime if not using delta input (mouse)
        if (!inputIsDelta) lookInput *= Time.deltaTime;

        // rotate character horizontally (around the Y axis)
        transform.Rotate(Vector3.up * lookInput.x);

        // rotate camera vertically (around the X axis) with clamp
        yaw += -lookInput.y * verticalModifier;
        yaw = Mathf.Clamp(yaw, -maxYaw, maxYaw);
        cam.transform.localRotation = Quaternion.Euler(yaw, 0, 0);
    }
}
