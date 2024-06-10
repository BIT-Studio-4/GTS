using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string title;
    public string description;
    public bool isInteractable;
    public string interactableText = "Press (X) To Interact"; // This is the default but can be changed in Inspector, using X here for an example we can change this later when we pick a default

    // This method is made to be overridden if interactable is toggled true
    // This is for anything the player will push a button on to do something with
    public virtual void Interact()
    {

    }
}
