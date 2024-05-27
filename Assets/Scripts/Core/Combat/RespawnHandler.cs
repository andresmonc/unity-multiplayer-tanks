using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private TankPlayer playerPrefab;
    [Header("Settings")]
    [SerializeField] private float keptCoinPercentage = 50f;

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
        int newCointAmount = (int)(player.Wallet.Coins.Value * (keptCoinPercentage / 100));
        Destroy(player.gameObject);
        StartCoroutine(RespawnPlayer(player.OwnerClientId, newCointAmount));
    }



    private IEnumerator RespawnPlayer(ulong OwnerClientId, int newCointAmount)
    {
        yield return null;
        TankPlayer playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPoint(), Quaternion.identity);
        playerInstance.Wallet.Coins.Value = newCointAmount;
        playerInstance.NetworkObject.SpawnAsPlayerObject(OwnerClientId);
    }

}
