using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private float spawnRate;

    private List<GameObject> shelves;
    public List<GameObject> Shelves {
        get => shelves;
        set
        {
            shelves = value;
        }
    }

    private float timeSinceSpawn;

    public static CustomerManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found another instance of CustomerManager!!");
        }
        Instance = this;

        shelves = new List<GameObject>();
    }

    void Update()
    {
        if (Time.time > timeSinceSpawn + spawnRate / Mathf.Clamp(shelves.Count, 1, Mathf.Infinity))
            SpawnCustomer();
    }

    void SpawnCustomer()
    {
        Debug.Log("Spawned Customer!!");
        timeSinceSpawn = Time.time;
    }
}
