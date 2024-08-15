using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] List<Transform> waypoints;

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

    public void SpawnCustomer(RandomSell targetItem)
    {
        Customer customer = Instantiate(customerPrefab, waypoints[0]).GetComponent<Customer>();
        customer.targetItem = targetItem;
        // send duplicate of waypoints list, otherwise customer fiddles with list and breaks everything
        customer.waypoints = new(waypoints); 
    }
}