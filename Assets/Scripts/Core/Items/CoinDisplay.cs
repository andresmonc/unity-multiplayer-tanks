using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CoinCollector coinCollector;

    void Update()
    {
        text.SetText(coinCollector.Coins.Value.ToString());
    }
}
