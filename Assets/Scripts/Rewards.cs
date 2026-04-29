using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards : MonoBehaviour
{
    public int DiaValue, GreenValue;
    public CountWave countWave;
    //public Sprite sprite;
    public Coin Dia;
    public Coin Green;
    public bool isIncrease = false;
     
    void Start()
    {
        Dia.CoinValue = DiaValue;
        Green.CoinValue = GreenValue;
    }
    void Update()
    {
        if (countWave == null)
        {
            countWave = FindObjectOfType<CountWave>();
        }

        if (countWave.isWinR10 && !isIncrease && countWave.isChangeBG)
        {
            isIncrease = true;
            Dia.CoinValue += 1000;
            Green.CoinValue += 5;
            StartCoroutine(waitIncrease());
        }
    }

    IEnumerator waitIncrease()
    {
        yield return new WaitForSeconds(2f);
        isIncrease = false;
    }
}
