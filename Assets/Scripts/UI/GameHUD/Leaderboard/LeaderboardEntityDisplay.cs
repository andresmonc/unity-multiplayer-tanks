using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;

    public ulong ClientId { get; private set; }
    private FixedString32Bytes playerName;
    public int Coins { get; private set; }

    public void Initialize(ulong clientId, FixedString32Bytes name, int coins)
    {
        this.ClientId = clientId;
        playerName = name;
        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins){
        Coins = coins;
        UpdateText();
    }


    private void UpdateText()
    {
        displayText.text = $"1. {playerName} ({Coins})";
    }

}
