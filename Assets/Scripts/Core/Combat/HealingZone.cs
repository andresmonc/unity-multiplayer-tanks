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


}
