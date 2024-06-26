using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Aiming : NetworkBehaviour
{
    [SerializeField] private Transform turretTransform;
    [SerializeField] private InputReader inputReader;

    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        Vector2 aimScreenPosition = inputReader.AimPositon;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y
            );
    }
}
