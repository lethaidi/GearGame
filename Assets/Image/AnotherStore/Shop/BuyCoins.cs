using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyCoins : MonoBehaviour
{
    public CoinSpawner coinSpawner;

    public void Buy()
    {
        if (coinSpawner != null)
        {
            coinSpawner.Spawn = true;
        }
    }
}
