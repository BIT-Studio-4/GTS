using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the movement of a player object. Adds gravity and handles move inputs.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField][Range(0.1f,2)] private float crouchDepth;
    [SerializeField][Range(0.1f,20)] private float crouchSpeed;
    [SerializeField] private Transform spawnpoint;

    private CharacterController cc;
    private InputAction moveAction;
    private Vector3 moveVector;
    private float standHeight;
    private float cameraTargetHeight;
    private Camera cam;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();

        standHeight = cam.transform.localPosition.y;
        cameraTargetHeight = standHeight;
    }

    void Start()
    {
        // returns Vector2 where x = left/right & y = up/down, on controller/keyboard
        moveAction = InputSystem.actions.FindAction("Move");
        // add listener to crouch event
        InputSystem.actions.FindAction("Crouch").performed += ctx => HandleCrouchInput();
        // start at spawnpoint, + half of player height because its pivot is in the center
        spawnpoint.position += Vector3.up * cc.height / 2;
        transform.position = spawnpoint.position;
    }

    void Update()
    {
        // handle walking movement (horizontal) ~ and prevent walking when in menu
        Vector2 moveInput = UIManager.Instance.IsGUIOpen ? Vector2.zero : moveAction.ReadValue<Vector2>() * moveSpeed;
        Vector2.ClampMagnitude(moveInput, moveSpeed);

        // transform.forward and transform.right are forward/back & left/right motion respectively
        // ~in relation to character rotation
        moveVector = transform.forward * moveInput.y + transform.right * moveInput.x
            + new Vector3(0, moveVector.y, 0);  // keep Y value the same as last frame

        if (cc.isGrounded)
            moveVector.y = 0;
        else
            // add gravity acceleration~ multiplying by deltaTime twice is NOT a mistake!!
            moveVector += Physics.gravity * Time.deltaTime;

        cc.Move(moveVector * Time.deltaTime);

        // move camera towards target (crouched or standing) height, with an exponential ease
        float cameraNewHeight = Mathf.Lerp(cam.transform.localPosition.y, cameraTargetHeight, crouchSpeed * Time.deltaTime);
        cam.transform.localPosition = new Vector3(
            cam.transform.localPosition.x,
            cameraNewHeight,
            cam.transform.localPosition.z);

        // respawn if player has fallen out of bounds
        if (transform.position.y < -1)
            transform.position = spawnpoint.position + Vector3.up * cc.height / 2;
    }

    void HandleCrouchInput()
    {
        if (cameraTargetHeight == standHeight)
            cameraTargetHeight = standHeight - crouchDepth;
        else
            cameraTargetHeight = standHeight;
    }
}
