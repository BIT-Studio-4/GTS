using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacementType
{
    Structure, 
    Stock
}

[System.Serializable]
public class PlaceableObject
{
    public string name;
    public GameObject prefab;
    public PlacementType type;
}
