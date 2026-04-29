using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int CoinValue, currentValue, StartCoin;
    public AudioSource audioSource;
    public bool isSave = false;

    private string saveKey; // key lưu riêng cho từng object

    void Start()
    {
        // Mỗi object sẽ có key riêng, ví dụ theo tên object
        saveKey = "CoinValue_" + gameObject.name;

        if (isSave)
        {
            CoinValue = PlayerPrefs.GetInt(saveKey, StartCoin);
        }
        else
        {
            CoinValue = StartCoin;
        }

        currentValue = CoinValue;
    }

    void Update()
    {
        PlaySound();
        text.text = FormatNumber(CoinValue);
    }

    void PlaySound()
    {
        if (currentValue != CoinValue)
        {
            if (audioSource != null) audioSource.Play();

            currentValue = CoinValue;
            if (isSave)
            {
                // Lưu tiền khi có thay đổi
                PlayerPrefs.SetInt(saveKey, CoinValue);
                PlayerPrefs.Save();
            }
        }
    }

    string FormatNumber(int num)
    {
        if (num >= 1000000000) // Tỉ
            return (num / 1000000000f).ToString("0.#") + "B";
        else if (num >= 1000000) // Triệu
            return (num / 1000000f).ToString("0.#") + "M";
        else if (num >= 1000) // Ngàn
            return (num / 1000f).ToString("0.#") + "K";
        else
            return num.ToString();
    }
}
