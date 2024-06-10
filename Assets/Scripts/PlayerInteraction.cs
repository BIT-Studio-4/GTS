using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;


//This script is to go onto the players camera
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance;
    [SerializeField] private TextMeshProUGUI interactionTextTitle;
    [SerializeField] private TextMeshProUGUI interactionTextDescription;
    [SerializeField] private TextMeshProUGUI interactionTextButton;

    private void Awake()
    {
        SetInteractionText("");
    }

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, interactionDistance))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

            InteractableObject interactableObject = hit.transform.GetComponent<InteractableObject>();

            if (interactableObject != null)
            {
                SetInteractionDisplay(interactableObject);
            }
            else
            {
                SetInteractionText(hit.transform.name);
            }
        }
        else
        {
            SetInteractionText("");
        }
    }

    private void SetInteractionDisplay(InteractableObject interactableObject)
    {
        if (interactableObject.isInteractable)
        {
            SetInteractionText(interactableObject.title, interactableObject.description, interactableObject.interactableText);
            
            //Do a check here when the interact key is pressed down once the input system is implimented
            //if (Check For Input)
            //{
            //    interactableObject.Interact();
            //}
        }
        else
        {
            SetInteractionText(interactableObject.title, interactableObject.description);
        }
    }

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
