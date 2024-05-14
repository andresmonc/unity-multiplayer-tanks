using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinCollector : NetworkBehaviour
{
    public NetworkVariable<int> Coins = new NetworkVariable<int>();

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
}
