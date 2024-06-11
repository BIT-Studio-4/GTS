using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private CharacterController cc;
    private InputAction moveAction;
    private Vector3 moveVector;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>() * moveSpeed;
        Vector2.ClampMagnitude(moveInput, moveSpeed);

        Vector3 moveHorizontal = transform.forward * moveInput.y + transform.right * moveInput.x;

        if (cc.isGrounded)
            moveVector.y = 0;
        else
            // add gravity acceleration~ multiplying by deltaTime twice is NOT a mistake!!
        moveVector += Physics.gravity * Time.deltaTime;

        cc.Move(moveVector * Time.deltaTime);
    }
}
