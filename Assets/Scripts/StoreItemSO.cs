using UnityEngine;

[CreateAssetMenu(fileName = "StoreItemSO", menuName = "ScriptableObjects/StoreItemSO", order = 1)]
public class StoreItemSO : ScriptableObject
{
    public string itemName;
    /// <summary>
    /// The id that lines up with the id in the database
    /// </summary>
    public int id;
    public GameObject prefab;
    public PlacementType type;
    public int cost;
}
