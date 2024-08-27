using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private Transform handTransform;

    [HideInInspector] public List<Transform> waypoints;
    [HideInInspector] public SellItem targetItem;
    private Vector3 targetPosition;
    private Vector3 moveVector;
    private bool atEndWaypoint;

    IEnumerator Start()
    {
        // walks through all waypoints (from outside store to target item)
        // then reverses the waypoints list to walk through in opposite order
        // and then die
        waypoints.Add(targetItem.transform);
        StartCoroutine(WalkThroughWaypoints());
        yield return new WaitUntil(() => atEndWaypoint);
        PickUpItem();
        waypoints.Reverse();
        StartCoroutine(WalkThroughWaypoints());
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
            targetPosition.y = 0;
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
    }
}
