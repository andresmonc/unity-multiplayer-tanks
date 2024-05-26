using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform leaderBoardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;
    [Header("Settings")]
    [SerializeField] private int entitiesToDisplay = 8;


    private NetworkList<LeaderboardEntity> leaderboardEntities;
    private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntity>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
            foreach (LeaderboardEntity entity in leaderboardEntities)
            {
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntity>
                {
                    Type = NetworkListEvent<LeaderboardEntity>.EventType.Add,
                    Value = entity
                });
            }
        }
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
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
        }
        if (!IsServer) { return; }
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        Debug.Log("Someone has spawned");
        leaderboardEntities.Add(new LeaderboardEntity
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            Coins = 0
        });
        player.Wallet.Coins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }
    private void HandlePlayerDespawned(TankPlayer player)
    {
        if (leaderboardEntities == null) { return; }
        foreach (LeaderboardEntity entity in leaderboardEntities)
        {
            if (entity.ClientId != player.OwnerClientId)
            {
                continue;
            }
            leaderboardEntities.Remove(entity);
            break;
        }
        player.Wallet.Coins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntity> changeEvent)
    {
        var changeClientId = changeEvent.Value.ClientId;
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntity>.EventType.Add:
                if (!entityDisplays.Any(x => x.ClientId == changeClientId))
                {
                    LeaderboardEntityDisplay display = Instantiate(leaderboardEntityPrefab, leaderBoardEntityHolder);
                    display.Initialize(changeEvent.Value.ClientId, changeEvent.Value.PlayerName, changeEvent.Value.Coins);
                    entityDisplays.Add(display);
                }
                break;
            case NetworkListEvent<LeaderboardEntity>.EventType.Remove:
                LeaderboardEntityDisplay displayToRemove = entityDisplays.FirstOrDefault(x => x.ClientId == changeClientId);
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove);
                    entityDisplays.Remove(displayToRemove);
                }
                break;
            case NetworkListEvent<LeaderboardEntity>.EventType.Value:
                LeaderboardEntityDisplay displayToUpdate = entityDisplays.FirstOrDefault(x => x.ClientId == changeClientId);
                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        };
        entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));
        for (int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();
            bool shouldShow = i <= entitiesToDisplay - 1;
            entityDisplays[i].gameObject.SetActive(shouldShow);
        }

        LeaderboardEntityDisplay myDisplay = entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);
        if (myDisplay != null)
        {
            if (myDisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
            {
                leaderBoardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
        }
    }

    private void HandleCoinsChanged(ulong clientId, int newCoins)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientId != clientId) { continue; }
            leaderboardEntities[i] = new LeaderboardEntity
            {
                ClientId = leaderboardEntities[i].ClientId,
                PlayerName = leaderboardEntities[i].PlayerName,
                Coins = newCoins
            };
            return;
        }
    }

}
