using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [System.Serializable]
    public class Path
    {
        public List<Transform> nodes;
    }
    
    public static CustomerManager Instance;

    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private List<Path> paths;
    [SerializeField] private float minSpawnCooldown;
    [SerializeField] private float maxSpawnCooldown;
    [SerializeField, Range(0f, 1f)] private float baseChanceOfEnter;
    [SerializeField] private Transform entranceNode;
    public float BaseChanceOfEnter { get => baseChanceOfEnter; }
    public Transform EntranceNode { get => entranceNode; }

    private List<GameObject> customers;
    public List<GameObject> Customers 
    {
        get => customers;
        set => customers = value;
    }
    private Coroutine customerSpawningLoop;
    private bool spawnCustomers = true;
    public bool SpawnCustomers 
    { 
        get => spawnCustomers;
        set  
        {
            spawnCustomers = value;
            if (spawnCustomers) customerSpawningLoop = StartCoroutine(SpawnCustomerLoop());
            else StopCoroutine(customerSpawningLoop);
        }

    }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found another instance of CustomerManager!!");
        }
        Instance = this;
    }

    private void Start()
    {
        SpawnCustomers = true;
    }

    private IEnumerator SpawnCustomerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnCooldown, maxSpawnCooldown));
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
