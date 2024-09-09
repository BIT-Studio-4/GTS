using System.Collections.Generic;
using UnityEngine;

public class GhostObjectPlacement : MonoBehaviour
{
    [SerializeField] private Material ghostMaterial;
    [Tooltip("How far away to check for intersecting objects")]
    [SerializeField] private float intersectionRadius = 1;

    public bool isIntersecting { get => CheckIntersection(); }
    public bool canBePlaced { get; set; }

    private PlayerInteraction playerInteraction;
    private MeshFilter meshFilter;
    private Animator animator;
    private MeshCollider meshCollider;

    void Awake()
    {
        playerInteraction = GetComponentInParent<PlayerInteraction>();
        meshFilter = GetComponent<MeshFilter>();
        animator = GetComponent<Animator>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void GetMeshFromHeldObject()
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
        meshCollider.sharedMesh = mesh;
    }

    public void UpdateTransform(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        animator.SetBool("canBePlaced", canBePlaced);
    }

    bool CheckIntersection()
    {
        // edited code snippet from https://docs.unity3d.com/ScriptReference/Physics.ComputePenetration.html
        Collider[] neighbours = Physics.OverlapSphere(transform.position, intersectionRadius);

        for (int i = 0; i < neighbours.Length; ++i)
        {
            var collider = neighbours[i];

            if (collider == meshCollider)
                continue; // skip ourself

            Vector3 otherPosition = collider.gameObject.transform.position;
            Quaternion otherRotation = collider.gameObject.transform.rotation;

            Vector3 direction;
            float distance;

            bool overlapped = Physics.ComputePenetration(
                meshCollider, transform.position, transform.rotation,
                collider, otherPosition, otherRotation,
                out direction, out distance
            );

            if (overlapped)
            {
                Debug.Log("intersecting with " + collider.gameObject.name);
                return true;
            }
        }

        return false;
    }
}
