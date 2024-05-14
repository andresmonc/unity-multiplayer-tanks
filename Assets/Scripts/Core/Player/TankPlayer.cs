using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;
    [field: SerializeField] public Health Health { get; private set; }

    private bool spawned = false;

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = HostSingleton.Instance.HostGameManager.NetworkServer.GetUseDataByClientID(OwnerClientId);
            PlayerName.Value = userData.userName;

            OnPlayerSpawned?.Invoke(this);
        }
        if (IsOwner)
        {
            cinemachineVirtualCamera.Priority = ownerPriority;
        }
    }

    private void Update()
    {
        if (!spawned && transform.position != Vector3.zero)
        {
            spawned = true;
        }
        if (!spawned)
        {
            Vector3 spawnPoint = SpawnPoint.GetRandomSpawnPoint();
            if (spawnPoint == Vector3.zero)
            {
                return;
            }
            transform.position = SpawnPoint.GetRandomSpawnPoint();
            spawned = true;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }

}