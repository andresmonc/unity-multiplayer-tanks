using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TankPlayer tankPlayer;
    [SerializeField] private TMP_Text displayNameText;
    private void Start()
    {
        HandlePlayerNameChanged(string.Empty, tankPlayer.PlayerName.Value);
        tankPlayer.PlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        displayNameText.text = newName.ToString();
    }

    private void OnDestroy()
    {
        tankPlayer.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }
}
