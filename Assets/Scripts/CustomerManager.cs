using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [System.Serializable]
    public class Path
    {
        public List<Transform> nodes;
    }

    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private List<Path> paths;
    [SerializeField, Range(0f, 1f)] private float baseChanceOfEnter;
    public float BaseChanceOfEnter { get => baseChanceOfEnter; }
    [SerializeField] private Transform entranceNode;
    public Transform EntranceNode { get => entranceNode; }

    private List<GameObject> customers;
    public List<GameObject> Customers 
    {
        get => customers;
        set => customers = value;
    }

    public static CustomerManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found another instance of CustomerManager!!");
        }
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (Random.Range(0, 250) == 0)
        {
            SpawnCustomer();
        }
    }

    // Picks a path for the customer to walk along, then spawns them at the beginning
    public void SpawnCustomer()
    {
        // Picks a random path to walk down
        Path path = GetRandomPath();

        Customer customer = Instantiate(customerPrefab, path.nodes[0].position, Quaternion.identity, transform).GetComponent<Customer>();
        // send duplicate of waypoints list, otherwise customer fiddles with list and breaks everything
        customer.waypoints = new(path.nodes); 
    }

    public Path GetRandomPath()
    {
        return paths[Random.Range(0, paths.Count)];
    }

    public SellItem PickItemToBuy()
    {
        if (StockManager.Instance.itemsToSell.Count == 0) return null;

        return StockManager.Instance.itemsToSell[Random.Range(0, StockManager.Instance.itemsToSell.Count)];
    }
}
