using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gem : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int CoinValue;

    void Update()
    {
        text.text = CoinValue.ToString();
    }
}
