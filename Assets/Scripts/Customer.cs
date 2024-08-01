using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [HideInInspector] public List<Transform> waypoints;
    private Vector3 targetPosition;
    [HideInInspector] public RandomSell targetItem;
    private Vector3 moveVector;

    IEnumerator Start()
    {
        waypoints.Add(targetItem.transform);
        foreach (Transform waypoint in waypoints)
        {
            targetPosition = waypoint.position;
            targetPosition.y = 0;
            yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPosition) < 0.5f);
        }
        targetItem.transform.parent = transform;
        waypoints.Reverse();
        foreach (Transform waypoint in waypoints)
        {
            targetPosition = waypoint.position;
            targetPosition.y = 0;
            yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPosition) < 0.5f);
        }
        targetItem.isSold = true;
        yield return null;
        Destroy(gameObject);
    }

    void Update()
    {
        transform.LookAt(targetPosition);
        moveVector = transform.forward * walkSpeed;
        transform.position += moveVector * Time.deltaTime;
    }
}
