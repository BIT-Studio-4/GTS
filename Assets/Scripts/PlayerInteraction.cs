using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// PlayerInteraction detects what the player is looking at and displays it to the player via the UI.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance;
    [SerializeField] private TextMeshProUGUI interactionTextTitle;
    [SerializeField] private TextMeshProUGUI interactionTextDescription;
    [SerializeField] private TextMeshProUGUI interactionTextButton;

    private void Start()
    {
        SetInteractionText("");
    }

    // Draws a raycast to whats in front of it and updates the UI depending on what it hits
    void FixedUpdate()
    {
        RaycastHit hit;

        // Sends a raycast and stores it in the hit variable
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, interactionDistance))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

            InteractableObject interactableObject = hit.transform.GetComponent<InteractableObject>();

            // Checks if the hit object has the InteractableObject script on it to update the text
            if (interactableObject != null)
            {
                if (interactableObject.IsInteractable)
                {
                    SetInteractionText(interactableObject.Title, interactableObject.Description, interactableObject.InteractableText);

                    // Do a check here when the interact key is pressed down once the input system is implimented
                    // if (Check For Input)
                    // {
                    //     interactableObject.Interact();
                    // }
                }
                else
                {
                    SetInteractionText(interactableObject.Title, interactableObject.Description);
                }
            }
            else
            {
                SetInteractionText("");
            }
        }
        else
        {
            SetInteractionText("");
        }
    }

    // Sets the various interaction texts, uses polymorphism to not have to write out all variables when not needed
    private void SetInteractionText(string title, string description, string button)
    {
        interactionTextTitle.text = title;
        interactionTextDescription.text = description;
        interactionTextButton.text = button;
    }

    private void SetInteractionText(string title, string description)
    {
        SetInteractionText(title, description, "");
    }

    private void SetInteractionText(string title)
    {
        SetInteractionText(title, "");
    }
}
