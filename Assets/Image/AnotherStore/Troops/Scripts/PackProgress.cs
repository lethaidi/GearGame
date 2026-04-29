using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PackProgress : MonoBehaviour
{
    public BannerPack  Pack;
    public Image image;
    public TextMeshProUGUI rank;
    public float tempExp;

    void Start()
    {
        tempExp = Pack.currentExp;
        UpdateUI();
    }
    
    void Update()
    {
        if (tempExp != Pack.currentExp)
        {
            tempExp = Pack.currentExp;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        float fillAmount = (float) Pack.currentExp / Pack.expToNextLevel;
        image.fillAmount = fillAmount;
        rank.text = Pack.packRank.ToString();
    }
}
