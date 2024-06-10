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

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>() * moveSpeed;
        Vector2.ClampMagnitude(moveInput, moveSpeed);

        moveVector = new Vector3(moveInput.x, moveVector.y, moveInput.y);

        moveVector += Physics.gravity * Time.deltaTime;

        cc.Move(moveVector * Time.deltaTime);
    }
}
