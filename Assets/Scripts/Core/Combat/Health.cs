using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{

    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();
    public Action<Health> DeathEvent;

    public bool isDead = false;
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }
        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ValueCheck(damageValue);
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int restoreValue)
    {
        ValueCheck(restoreValue);
        ModifyHealth(restoreValue);
    }

    private void ModifyHealth(int value)
    {
        if (isDead)
        {
            return;
        }
        int newHealth = CurrentHealth.Value + value;
        Debug.Log("New health would be: " + newHealth);
        CurrentHealth.Value = Math.Clamp(newHealth, 0, MaxHealth);
        Debug.Log("Clamped Health: " + newHealth);
        if (newHealth == 0)
        {
            Debug.Log("Death Event Invoked!");
            DeathEvent?.Invoke(this);
            isDead = true;
        }
    }

    private void ValueCheck(int value)
    {
        if (value < 0)
        {
            throw new System.Exception("Only positive numbers are allowed");
        }
    }
}
