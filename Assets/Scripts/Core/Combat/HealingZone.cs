using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Netcode;
using UnityEngine;

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

    // List<TankPlayer> playersInZone = new List<TankPlayer>();
    HashSet<TankPlayer> playersInZone = new HashSet<TankPlayer>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) { return; }
        if (other.TryGetComponent<TankPlayer>(out TankPlayer tankPlayer))
        {
            playersInZone.Add(tankPlayer);
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer) { return; }
        if (other.TryGetComponent<TankPlayer>(out TankPlayer tankPlayer))
        {
            playersInZone.Remove(tankPlayer);
        }
    }
}
