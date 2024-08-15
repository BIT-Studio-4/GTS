using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public class GhostObjectPlacement : MonoBehaviour
{
    private PlayerInteraction playerInteraction;
    private GameObject ghostObject;
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private Transform ghostObjectParent;

    void Awake()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
    }

    void Start()
    {
        InventoryManager.Instance.OnHeldObjectChange += HandleObjectChanged;
        HandleObjectChanged(null);
    }

    void Update()
    {
        if (ghostObject == null) return;

        if (!playerInteraction.raycastHasHit || InventoryManager.Instance.HeldObject == null)
        {
            ghostObject.SetActive(false);
            return;
        }

        ghostObject.SetActive(true);
        ghostObject.transform.position = playerInteraction.Hit.point;
        ghostObject.transform.LookAt(playerInteraction.transform);
        Vector3 rotation = ghostObject.transform.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        ghostObject.transform.rotation = Quaternion.Euler(rotation);
    }

    void HandleObjectChanged(PlaceableObject heldObject)
    {
        if (heldObject == null)
        {
            Destroy(ghostObject);
            return;
        }

        ghostObject = Instantiate(heldObject.prefab, ghostObjectParent);
        ghostObject.name = $"Ghost {ghostObject.name}";
        YassifyGhostObject();
    }

    void YassifyGhostObject()
    {
        Collider[] colliders = ghostObject.GetComponentsInChildren<Collider>();
        Debug.Log($"{colliders}: {colliders.Length}");
        MeshRenderer[] meshRenderers = ghostObject.GetComponentsInChildren<MeshRenderer>();
        Debug.Log($"{meshRenderers}: {meshRenderers.Length}");

        foreach (Collider collider in colliders) collider.enabled = false;
        foreach (MeshRenderer meshRenderer in meshRenderers)
            foreach (Material material in meshRenderer.materials)
                material.CopyPropertiesFromMaterial(ghostMaterial);
    }
}
