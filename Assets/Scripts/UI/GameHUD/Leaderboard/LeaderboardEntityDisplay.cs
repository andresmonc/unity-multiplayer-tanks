using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    // TODO: SET THIS
    [SerializeField] private Color myColor;

    public ulong ClientId { get; private set; }
    private FixedString32Bytes playerName;
    public int Coins { get; private set; }

    public void Initialize(ulong clientId, FixedString32Bytes name, int coins)
    {
        this.ClientId = clientId;
        playerName = name;
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            displayText.Color = myColor;
        }
        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }


    public void UpdateText(int order)
    {
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {playerName} ({Coins})";
    }

}
