using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private Transform handTransform;
    [HideInInspector] public List<Transform> waypoints;
    private Vector3 targetPosition;
    [HideInInspector] public RandomSell targetItem;
    private Vector3 moveVector;
    private bool atEndWaypoint;

    IEnumerator Start()
    {
        waypoints.Add(targetItem.transform);
        StartCoroutine(WalkThroughWaypoints());
        yield return new WaitUntil(() => atEndWaypoint);
        PickUpItem();
        waypoints.Reverse();
        StartCoroutine(WalkThroughWaypoints());
        yield return new WaitUntil(() => atEndWaypoint);
        yield return null;
        Destroy(gameObject);
    }

    void Update()
    {
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
        targetItem.isSold = true;
        targetItem.transform.parent = transform;
        targetItem.transform.position = handTransform.position;
        targetItem.transform.rotation = handTransform.rotation;
    }
}
