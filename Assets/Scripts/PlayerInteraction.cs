using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerInteraction detects what the player is looking at and displays it to the player via the UI.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance;
    [SerializeField] private TextMeshProUGUI interactionTextTitle;
    [SerializeField] private TextMeshProUGUI interactionTextDescription;
    [SerializeField] private TextMeshProUGUI interactionTextButton;

    private RaycastHit hit;
    public RaycastHit Hit { get => hit; }
    public bool raycastHasHit { get; private set; }
    private InputAction interactAction;

    private void Start()
    {
        SetInteractionText("");
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        raycastHasHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, interactionDistance);
    }

    // Draws a raycast to whats in front of it and updates the UI depending on what it hits
    void FixedUpdate()
    {
        // Sends a raycast and stores it in the hit variable
        if (raycastHasHit)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

            if (hit.transform == null)
            {
                Debug.Log("it WAS null!!!!!!!!!");
                return;
            }

            InteractableObject interactableObject = hit.transform.GetComponent<InteractableObject>();

            // Checks if the hit object has the InteractableObject script on it to update the text
            if (interactableObject != null)
            {
                if (interactableObject.IsInteractable)
                {
                    SetInteractionText(interactableObject.Title, interactableObject.Description, interactableObject.InteractableText);

                    // Calling the overrideable Interact method on the interactable object
                    if (interactAction.IsPressed())
                    {
                        interactableObject.Interact();
                    }
                }
                else
                {
                    SetInteractionText(interactableObject.Title, interactableObject.Description);
                }
            }
            else
            {
                SetInteractionText();
            }
        }
        else
        {
            SetInteractionText();
        }
    }

    // Sets the various interaction texts, uses polymorphism to not have to write out all variables when not needed
    private void SetInteractionText(string title = "", string description = "", string button = "")
    {
        interactionTextTitle.text = title;
        interactionTextDescription.text = description;
        interactionTextButton.text = button;
    }
}