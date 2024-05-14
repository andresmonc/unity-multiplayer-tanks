using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{

    [SerializeField] private NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        TankPlayer[] players = FindObjectsOfType<TankPlayer>();
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDeSpawned;
    }



    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDeSpawned;
    }


    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.Health.DeathEvent += (ignored) => HandlePlayerDeath(player);
    }

    private void HandlePlayerDeSpawned(TankPlayer player)
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
        NetworkObject playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPoint(), Quaternion.identity);
        playerInstance.SpawnAsPlayerObject(OwnerClientId);
    }

}
