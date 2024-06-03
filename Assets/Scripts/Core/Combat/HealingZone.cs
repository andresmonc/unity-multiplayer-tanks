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

    private NetworkVariable<int> HealPower = new NetworkVariable<int>();

    private float remainingCoolDown;
    private float tickTimer;
    private void Update()
    {
        if (!IsServer) { return; }

        if (remainingCoolDown > 0f)
        {
            remainingCoolDown -= Time.deltaTime;
            if (remainingCoolDown <= 0f)
            {
                HealPower.Value = maxHealPower;
            }
            else { return; }
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= 1 / healTickRate)
        {
            foreach (TankPlayer player in playersInZone)
            {
                Debug.Log("healing a player");
                if (HealPower.Value == 0)
                {
                    Debug.Log("healpower at 0");
                    break;
                }
                if (player.Health.CurrentHealth.Value == player.Health.MaxHealth)
                {
                    Debug.Log("Not healing, already full health");
                    continue;
                }
                if (player.Wallet.Coins.Value < coinsPerTick)
                {
                    Debug.Log("not enough coins!");
                    continue;
                }
                player.Wallet.SpendCoins(coinsPerTick);
                player.Health.RestoreHealth(healthPerTick);
                HealPower.Value -= 1;
                if (HealPower.Value == 0)
                {
                    remainingCoolDown = healCooldown;
                }
            }
            tickTimer = tickTimer % (1 / healTickRate);
        }

    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }

    public override void OnNetworkSpawn()
    {

        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, HealPower.Value);
        }

        if (IsServer)
        {
            HealPower.Value = maxHealPower;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) { return; }
        if (other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer tankPlayer))
        {
            Debug.Log("adding aplayer to zone");
            playersInZone.Add(tankPlayer);
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

    private void HandleHealPowerChanged(int oldHeal, int newHealPower)
    {
        healPowerBar.fillAmount = (float)newHealPower / maxHealPower;
    }
}
