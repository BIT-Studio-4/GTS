using UnityEngine;

public class GhostObjectPlacement : MonoBehaviour
{
    [SerializeField] private Material ghostMaterial;

    private PlayerInteraction playerInteraction;
    private MeshFilter meshFilter;
    private Animator animator;

    void Awake()
    {
        playerInteraction = GetComponentInParent<PlayerInteraction>();
        meshFilter = GetComponent<MeshFilter>();
        Debug.Log(meshFilter);
        animator = GetComponent<Animator>();
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
    }

    public void UpdateTransform(Vector3 position, Quaternion rotation, bool canBePlaced)
    {
        transform.position = position;
        transform.rotation = rotation;
        animator.SetBool("canBePlaced", canBePlaced);
    }
}
