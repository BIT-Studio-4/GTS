using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody rb;
    private InputAction moveAction;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();



        //rb.velocity = transform.forward * moveValue * moveSpeed * Time.deltaTime;

        Vector3 movement = new Vector3(moveValue.x * moveSpeed * Time.deltaTime, 0, moveValue.y * moveSpeed * Time.deltaTime);

        transform.position += movement;
    }
}
