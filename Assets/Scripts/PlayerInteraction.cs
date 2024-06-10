using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This script is to go onto the players camera
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance;

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, interactionDistance))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
        }
    }
}
