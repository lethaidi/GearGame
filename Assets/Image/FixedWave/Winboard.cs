using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Winboard : MonoBehaviour
{
    [Header("Tên object chứa Coin")]
    public string StartDiaName;
    public string StartGreenName;

    public Coin StartDia;
    public Coin StartGreen;

    [Header("Coin Win (Kéo thả thủ công)")]
    public Coin WinDia;
    public Coin WinGreen;

    public int currentDia , currentGreen;
    public bool isFind = false;
    public ExperienceBar experienceBar;
    public TextMeshProUGUI expRewards;

    void Update()
    {
        FindObj(); // Tìm object chứa Coin
        if (StartDia != null && WinDia != null)
        {
            WinDia.CoinValue = currentDia;
        }
        if (StartGreen != null && WinGreen != null)
        {
            WinGreen.CoinValue = currentGreen;
        }
    }
    void FindObj()
    {
        if(isFind) return; // Tránh tìm lại nếu đã tìm rồi
        if (!string.IsNullOrEmpty(StartDiaName))
        {
            GameObject obj = GameObject.Find(StartDiaName);
            if (obj != null) StartDia = obj.GetComponent<Coin>();
            currentDia = StartDia.CoinValue;
        }

        if (!string.IsNullOrEmpty(StartGreenName))
        {
            GameObject obj = GameObject.Find(StartGreenName);
            if (obj != null) StartGreen = obj.GetComponent<Coin>();
            currentGreen = StartGreen.CoinValue;
        }
        isFind = true;
        experienceBar = FindObjectOfType<ExperienceBar>();
        if (experienceBar != null)
        {
            int randomExp = Random.Range(150, 200);
            experienceBar.AddExp(randomExp);
            if (expRewards != null)
                expRewards.text = "+ " + randomExp.ToString() + " EXP";
        }
    }
}
