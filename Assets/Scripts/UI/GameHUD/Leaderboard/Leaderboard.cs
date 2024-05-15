using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderBoardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;

    private NetworkList<LeaderboardEntity> leaderboardEntities;

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntity>();
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {

    }
    private void HandlePlayerDespawned(TankPlayer player)
    {

    }



}
