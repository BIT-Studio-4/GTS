using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private Transform handTransform;
    [SerializeField] private TextMeshPro displayText;

    [HideInInspector] public List<Transform> waypoints;
    [HideInInspector] public SellItem targetItem;
    private Vector3 targetPosition;
    private Vector3 moveVector;
    private bool atEndWaypoint;

    IEnumerator Start()
    {
        // Walk to shop
        StartCoroutine(WalkThroughWaypoints());
        yield return new WaitUntil(() => atEndWaypoint);

        // Null if there is nothing to buy
        targetItem = CustomerManager.Instance.PickItemToBuy();

        // If going to buy something
        if (ShouldPickupItem(targetItem))
        {
            displayText.text = "customer";
            StockManager.Instance.itemPickedByCustomer.Invoke(targetItem);
            // Walk to entrance then the item to purchase
            waypoints.Clear();
            waypoints.Add(CustomerManager.Instance.EntranceNode);
            waypoints.Add(targetItem.transform);
            StartCoroutine(WalkThroughWaypoints());

            // Picks up item and leaves the shop
            yield return new WaitUntil(() => atEndWaypoint);
            PickUpItem();
            waypoints.Reverse();
            StartCoroutine(WalkThroughWaypoints());
        }

        // Walk along new reversed path to leave
        yield return new WaitUntil(() => atEndWaypoint);
        waypoints.Clear();
        List<Transform> path = new (CustomerManager.Instance.GetRandomPath().nodes);
        waypoints.AddRange(path);
        waypoints.Reverse();
        StartCoroutine(WalkThroughWaypoints());

        // Kill when at end of new path
        yield return new WaitUntil(() => atEndWaypoint);
        Destroy(gameObject);
    }

    void Update()
    {
        // simple method to get to target position
        // look at it and move forward
        transform.LookAt(targetPosition);
        moveVector = transform.forward * walkSpeed;
        transform.position += moveVector * Time.deltaTime;
    }

    IEnumerator WalkThroughWaypoints()
    {
        atEndWaypoint = false;
        foreach (Transform waypoint in waypoints)
        {
            targetPosition = waypoint.position;
            targetPosition.y = 0.15f;
            yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPosition) < 0.5f);
        }
        atEndWaypoint = true;
    }

    void PickUpItem()
    {
        GameManager.Instance.Money += targetItem.moneyOnSell;
        // puts the item in customer's hand
        // this also parents the item to the customer so when the customer is destroyed
        // so too is the item
        targetItem.transform.parent = transform;
        targetItem.transform.position = handTransform.position;
        targetItem.transform.rotation = handTransform.rotation;

        StockManager.Instance.itemSold.Invoke(targetItem);
        if (TutorialManager.Instance.InProgress) TutorialManager.Instance.CompleteTutorialTask("soldStock");
    }

    /// <summary>
    /// Determines if the customer should go and pickup an item or not
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool ShouldPickupItem(SellItem item)
    {
        bool isNotNull = item != null;
        bool randomChance = Random.Range(0f, 1f) <= CustomerManager.Instance.BaseChanceOfEnter;
        return isNotNull && randomChance;
    }
}
