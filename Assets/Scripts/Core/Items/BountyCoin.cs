using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyCoin : Coin
{
    public override int Collect()
    {
        if (!IsServer || alreadyCollected)
        {
            Show(false);
            return 0;
        }
        alreadyCollected = true;
        Destroy(gameObject);
        return coinValue;
    }
}
