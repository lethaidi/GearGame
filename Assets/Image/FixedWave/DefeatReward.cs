using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatReward : MonoBehaviour
{
    public Coin Dia;
    public Coin Green;
    public bool isDefeat = false; // Cờ để kiểm tra đã thua hay chưa

    public CountWave countWave;
    void Update()
    {
        randomValue();
    }

    void randomValue()
    {
        if (isDefeat) return; // Nếu đã thua thì không làm gì cả
        isDefeat = true; // Đánh dấu đã thua
        Dia.CoinValue = Random.Range(1, 21) * 100;
        Green.CoinValue = Random.Range(1, 5);
        countWave.isWinR10 = true;
    }
}
