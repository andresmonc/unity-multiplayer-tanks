using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{

    [SerializeField] private TankPlayer playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }



    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }


    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.Health.DeathEvent += (ignored) => HandlePlayerDeath(player);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.Health.DeathEvent -= (ignored) => HandlePlayerDeath(player);
    }

    private void HandlePlayerDeath(TankPlayer player)
    {
        Destroy(player.gameObject);
        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }



    private IEnumerator RespawnPlayer(ulong OwnerClientId)
    {
        yield return null;
        TankPlayer playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPoint(), Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(OwnerClientId);
    }

}
