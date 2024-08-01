using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private float timeBetweenSpawns;
    [Header("Waypoints")]
    [SerializeField] private GameObject spawnpoint;
    [SerializeField] private GameObject storeEntrance;

    private List<GameObject> shelves;
    public List<GameObject> Shelves
    {
        get => shelves;
        set => shelves = value;
    }

    private List<GameObject> customers;
    public List<GameObject> Customers 
    {
        get => customers;
        set => customers = value;
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
        if (Time.time > timeSinceSpawn + timeBetweenSpawns / Mathf.Clamp(shelves.Count, 1, Mathf.Infinity))
            SpawnCustomer();
    }

    void SpawnCustomer()
    {
        Debug.Log("Spawned Customer!!");
        GameObject customer = Instantiate(customerPrefab, spawnpoint.transform);
        timeSinceSpawn = Time.time;
    }
}
