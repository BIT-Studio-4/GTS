using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An enum to determine if a placeable object is a structure (e.g a shelf) or a stock item (e.g a can)
public enum PlacementType
{
    Stock,
    Structure,
}

[System.Serializable]
public class PlaceableObject
{
    public string name;
    public GameObject prefab;
    public PlacementType type;
}
