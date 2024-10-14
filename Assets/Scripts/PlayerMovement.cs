using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the movement of a player object. Adds gravity and handles move inputs.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField][Range(1, 2)] private float sprintMultiplier;
    [SerializeField][Range(1, 10)] private float jumpForce;
    [SerializeField][Range(0, 1)] private float jumpBufferTime; // used when jump is pressed before touching the ground
    [SerializeField][Range(0.1f, 2)] private float crouchDepth;
    [SerializeField][Range(0.1f, 20)] private float crouchAnimationSpeed;
    [SerializeField][Range(0.01f, 1)] private float crouchMoveSpeedMultiplier;
    [SerializeField] private Transform spawnpoint;

    private CharacterController cc;
    private InputAction moveAction;
    private InputAction sprintAction;
    private Vector3 moveVector;
    private float standHeight;
    private float cameraTargetHeight;
    private Camera cam;
    private float jumpLastPressedTime;
    private bool isCrouched;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();

        standHeight = cam.transform.localPosition.y;
        cameraTargetHeight = standHeight;
        isCrouched = false;
        jumpLastPressedTime = Mathf.NegativeInfinity;
    }

    void Start()
    {
        // moveAction returns Vector2 where x = left/right & y = up/down, on controller/keyboard
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        // add listener to crouch event
        InputSystem.actions.FindAction("Crouch").performed += ctx => HandleCrouchInput();
        InputSystem.actions.FindAction("Jump").performed += ctx => HandleJumpInput();
        // start at spawnpoint, + half of player height because its pivot is in the center
        spawnpoint.position += Vector3.up * cc.height / 2;
        transform.position = spawnpoint.position;
    }

    void Update()
    {
        // handle walking movement (horizontal) ~ and prevent walking when in menu
        Vector2 moveInput = UIManager.Instance.IsGUIOpen ? Vector2.zero : moveAction.ReadValue<Vector2>() * moveSpeed;
        if (isCrouched) moveInput *= crouchMoveSpeedMultiplier;
        Vector2.ClampMagnitude(moveInput, moveSpeed);

        // transform.forward and transform.right are forward/back & left/right motion respectively
        // ~in relation to character rotation
        moveVector = transform.forward * moveInput.y + transform.right * moveInput.x
            + new Vector3(0, moveVector.y, 0);  // keep Y value the same as last frame

        Sprint();

        if (cc.isGrounded)
        {
            moveVector.y = 0;
            if (Time.time <= jumpLastPressedTime + jumpBufferTime)
                Jump();
        }
        else
            // add gravity acceleration~ multiplying by deltaTime twice is NOT a mistake!!
            moveVector += Physics.gravity * Time.deltaTime;

        cc.Move(moveVector * Time.deltaTime);

        // move camera towards target (crouched or standing) height, with an exponential ease
        float cameraNewHeight = Mathf.Lerp(cam.transform.localPosition.y, cameraTargetHeight, crouchAnimationSpeed * Time.deltaTime);
        cam.transform.localPosition = new Vector3(
            cam.transform.localPosition.x,
            cameraNewHeight,
            cam.transform.localPosition.z);

        // respawn if player has fallen out of bounds
        if (transform.position.y < -1)
            transform.position = spawnpoint.position + Vector3.up * cc.height / 2;
    }

    void Sprint()
    {
        if (!sprintAction.inProgress) return;
        moveVector.x *= 2;
        moveVector.z *= 2;
    }

    void Jump()
    {
        if (UIManager.Instance.IsGUIOpen) return;
        moveVector.y = jumpForce;
        jumpLastPressedTime = Mathf.NegativeInfinity;
    }

    void HandleCrouchInput()
    {
        if (UIManager.Instance.IsGUIOpen) return;
        if (cameraTargetHeight == standHeight)
            cameraTargetHeight = standHeight - crouchDepth;
        else
            cameraTargetHeight = standHeight;
        isCrouched = !isCrouched;
    }
    
    void HandleJumpInput()
    {
        if (UIManager.Instance.IsGUIOpen) return;
        jumpLastPressedTime = Time.time;
    }
}
