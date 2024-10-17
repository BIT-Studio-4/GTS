using UnityEngine;

// An enum to determine if a placeable object is a structure (e.g a shelf) or a stock item (e.g a can)
public enum PlacementType
{
    Stock,
    Structure,
}

// The Placeable Object class which stores information on each placeable object like cans and boxes
[System.Serializable]
public class PlaceableObject
{
    public string name;
    /// <summary>
    /// The id that lines up with the id in the database
    /// </summary>
    public int id;
    public StoreItemSO storeItem;
    public GameObject prefab;
    public PlacementType type;
    public int count;
    public float salePrice; //added field for sale price

    public PlaceableObject(string name, int id, StoreItemSO storeItem, GameObject prefab, PlacementType type, int count = 0)
    {
        this.name = name;
        this.id = id;
        this.storeItem = storeItem;
        this.prefab = prefab;
        this.type = type;
        this.count = count;
    }
}
