using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInteraction))]
public class GhostObjectPlacement : MonoBehaviour
{
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private GameObject ghostObject;

    private PlayerInteraction playerInteraction;
    private MeshFilter meshFilter;
    private Animator animator;

    void Awake()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
        meshFilter = ghostObject.GetComponent<MeshFilter>();
        animator = GetComponent<Animator>();
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
            ghostObject.SetActive(false);
            return;
        }

        GetMeshFromHeldObject();
        ghostObject.SetActive(true);
    }

    void GetMeshFromHeldObject()
    {
        // code snippet from https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
        // combine all submeshes into one
        MeshFilter[] meshFilters = InventoryManager.Instance.HeldObject.prefab.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].mesh.SetTriangles(combine[i].mesh.triangles, 0);
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        meshFilter.sharedMesh = mesh;
    }
}
