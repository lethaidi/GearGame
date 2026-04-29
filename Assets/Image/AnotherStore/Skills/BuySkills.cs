using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuySkills : MonoBehaviour
{
    public ScriptableSkills scriptSkill;
    public TextMeshProUGUI textPrice;
    public int price;
    public Coin coin;
    public GameObject Grey, Green;
    public bool isBuy = false, canBuy = false;

    [Header("Các skill cần mua trước")]
    public List<BuySkills> requiredSkills;

    private string saveKey;
    private Color defaultColor;

    public AudioSource audioSource;

    void Start()
    {
        saveKey = "Skill_" + gameObject.name;
        isBuy = PlayerPrefs.GetInt(saveKey, 0) == 1;

        if (isBuy)
        {
            Grey.SetActive(false);
            Green.SetActive(true);
        }

        defaultColor = textPrice.color; // lưu màu gốc
        textPrice.text = price.ToString();
        UpdateCanBuy();
    }

    void Update()
    {
        UpdateCanBuy();
    }

    void UpdateCanBuy()
    {
        // Kiểm tra nếu skill yêu cầu đã mua hết
        bool allRequiredBought = true;
        if (requiredSkills != null && requiredSkills.Count > 0)
        {
            foreach (var req in requiredSkills)
            {
                if (req != null && !req.isBuy)
                {
                    allRequiredBought = false;
                    break;
                }
            }
        }

        // Chỉ có thể mua nếu đủ tiền và đã mua hết skill yêu cầu
        if (coin.CoinValue >= price && allRequiredBought && !isBuy)
        {
            canBuy = true;
            textPrice.color = defaultColor;
        }
        else
        {
            canBuy = false;
            // Nếu chưa đủ điều kiện mua (thiếu tiền hoặc skill yêu cầu chưa mua)
            if (!allRequiredBought)
                textPrice.color = Color.red;
            else if (coin.CoinValue < price)
                textPrice.color = Color.yellow; // ví dụ: vàng = thiếu tiền
        }
    }

    public void Buy()
    {
        if (audioSource != null)
            audioSource.Play();

        if (scriptSkill == null || !canBuy) return;

        // Thực hiện mua
        scriptSkill.valueSkill++;
        coin.CoinValue -= price;

        Grey.SetActive(false);
        Green.SetActive(true);
        isBuy = true;

        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();

        UpdateCanBuy();
    }
}
