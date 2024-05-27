using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinCollector : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private BountyCoin coinPrefab;
    [SerializeField] private Health health;
    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private float bountyPercentage = 50f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minCoinsForBounty = 5;
    [SerializeField] private LayerMask layerMask;
    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;
    public NetworkVariable<int> Coins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        health.DeathEvent += HandleDeath;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        health.DeathEvent -= HandleDeath;
    }

    private void HandleDeath(Health health)
    {
        int bountyValue = (int)(Coins.Value * (bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount;
        if (bountyCoinCount < minCoinsForBounty) { return; }
        for (int i = 0; i < bountyCoinCount; i++)
        {
            Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Coin>(out Coin coin))
        {
            int coinValue = coin.Collect();
            if (!IsServer)
            {
                return;
            }
            Coins.Value += coinValue;
        }
    }

    public void SpendCoins(int coinCount)
    {
        if (!IsServer)
        {
            return;
        }
        Coins.Value -= coinCount;
    }

    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
