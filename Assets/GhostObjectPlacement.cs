using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public class GhostObjectPlacement : MonoBehaviour
{
    private PlayerInteraction playerInteraction;
    [SerializeField] private GameObject ghostObject;

    void Awake()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
    }

    void Update()
    {
        if (!playerInteraction.raycastHasHit || InventoryManager.Instance.HeldObject == null)
        {
            ghostObject.SetActive(false);
            return;
        }

        ghostObject.transform.position = playerInteraction.Hit.point;
        ghostObject.transform.LookAt(playerInteraction.transform);
        Vector3 rotation = ghostObject.transform.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        ghostObject.transform.rotation = Quaternion.Euler(rotation);
        ghostObject.SetActive(true);
    }
}
