using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image healPowerBar;
    [Header("Settings")]
    [SerializeField] private int maxHealPower = 30;
    [SerializeField] private float healCooldown = 30f;
    [SerializeField] private float healTickRate = 1f;
    [SerializeField] private int coinsPerTick = 1;
    [SerializeField] private int healthPerTick = 1;

    HashSet<TankPlayer> playersInZone = new HashSet<TankPlayer>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger enter");
        if (!IsServer) { return; }
        if (other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer tankPlayer))
        {
            Debug.Log($"{tankPlayer.PlayerName}");
            playersInZone.Add(tankPlayer);
            Debug.Log(playersInZone.ToString());
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer) { return; }
        if (other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer tankPlayer))
        {
            playersInZone.Remove(tankPlayer);
            Debug.Log(playersInZone.ToString());
        }
    }
}
