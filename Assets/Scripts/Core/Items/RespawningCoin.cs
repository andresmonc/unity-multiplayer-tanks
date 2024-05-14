using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    private Vector3 previousPosition;

    private void Update()
    {
        if (previousPosition != transform.position)
        {
            Show(true);
        }
        previousPosition = transform.position;
    }
    public override int Collect()
    {
        if (!IsServer || alreadyCollected)
        {
            Show(false);
            return 0;
        }
        alreadyCollected = true;
        CollectEvent();
        return coinValue;
    }

}
