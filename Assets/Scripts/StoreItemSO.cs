using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoreItemSO", menuName = "ScriptableObjects/StoreItemSO", order = 1)]
public class StoreItemSO : ScriptableObject
{
    public string itemName;
    public GameObject prefab;
    public PlacementType type;
    public int cost;
}
