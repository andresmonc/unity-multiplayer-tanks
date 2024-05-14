using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    protected int coinValue = 10;
    protected bool alreadyCollected;

    public event Action<Coin> OnCollected;

    public abstract int Collect();

    public void CollectEvent()
    {
        OnCollected?.Invoke(this);
    }

    public void SetValue(int value)
    {
        coinValue = value;
    }

    protected void Show(bool show)
    {
        spriteRenderer.enabled = show;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
