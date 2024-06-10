using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private CharacterController cc;
    private InputAction moveAction;
    private Vector3 moveVector;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        moveVector = new Vector3(moveInput.x * moveSpeed, moveVector.y, moveInput.y * moveSpeed);

        moveVector += Physics.gravity * Time.deltaTime;

        cc.Move(moveVector * Time.deltaTime);
    }
}
