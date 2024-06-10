using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


//This script is to go onto the players camera
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance;
    [SerializeField] private TextMeshProUGUI interactionTextTitle;
    [SerializeField] private TextMeshProUGUI interactionTextDescription;

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
                SetInteractionText(interactableObject.title, interactableObject.description);
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

    private void SetInteractionText(string title, string description)
    {
        interactionTextTitle.text = title;
        interactionTextDescription.text = description;
    }

    private void SetInteractionText(string title)
    {
        SetInteractionText(title, "");
    }
}
