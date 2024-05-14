using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rbody;
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float turningRate = 30f;

    private Vector2 previousMovementVector;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }
        inputReader.MoveEvent -= HandleMove;
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        bodyTransform.Rotate(0f, 0f, previousMovementVector.x * -turningRate * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        rbody.velocity = (Vector2)bodyTransform.up * previousMovementVector.y * movementSpeed;
    }

    private void HandleMove(Vector2 vector)
    {
        previousMovementVector = vector;
    }
}
