using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonUpgrade : MonoBehaviour
{
    public Troops troops;
    public TextMeshProUGUI Price;
    public int priceUpgrade;
    public Image buttonImage;
    public Sprite greenSprite;
    public Sprite greySprite;
    public Coin coin;
    private string priceKey; // key lưu riêng cho từng object
    public AudioSource audioSource;
    public TaskSO taskUpgrade;

    [Header("Trạng thái nâng cấp")]
    public bool canUpgrade = false, isChangeColor = true; // true khi đủ tiền

    void Start()
    {
        int defaultPrice = 500;
        priceKey = "PriceUpgrade_" + gameObject.name;

        priceUpgrade = PlayerPrefs.GetInt(priceKey, defaultPrice);

        if (troops != null && Price != null)
        {
            Price.text = FormatNumber(priceUpgrade);
        }

        UpdateButtonState();
    }

    void Update()
    {
        UpdateButtonState();
    }

    public void Upgrade()
    {
        // Kiểm tra có thể nâng cấp không
        if (!canUpgrade) return;

        // Cập nhật tiến độ task
        if (taskUpgrade != null && taskUpgrade.currentValue < taskUpgrade.targetValue)
        {
            taskUpgrade.currentValue++;
        }

        // Âm thanh
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Trừ xu
        coin.CoinValue -= priceUpgrade;
        PlayerPrefs.SetInt("PlayerCoins", coin.CoinValue);

        // Gọi upgrade của troops
        if (troops != null)
        {
            troops.isUpgrade = true;
        }

        // Tăng giá ngẫu nhiên cho lần sau
        priceUpgrade += Random.Range(100, 500);
        PlayerPrefs.SetInt(priceKey, priceUpgrade);
        PlayerPrefs.Save();

        // Cập nhật UI
        if (Price != null)
        {
            Price.text = FormatNumber(priceUpgrade);
        }

        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        int playerCoins = coin.CoinValue;

        // Cập nhật biến trạng thái
        canUpgrade = playerCoins >= priceUpgrade;

        // Cập nhật hình ảnh nút
        if (buttonImage != null)
        {
            buttonImage.sprite = canUpgrade ? greenSprite : greySprite;
        }

        // Nếu muốn thay đổi màu giá tiền:
        if (Price != null && isChangeColor)
        {
            Price.color = canUpgrade ? Color.white : Color.red;
        }
    }

    string FormatNumber(int number)
    {
        if (number >= 1000000000)
            return (number / 1000000000f).ToString("0.#") + "B";
        if (number >= 1000000)
            return (number / 1000000f).ToString("0.#") + "M";
        if (number >= 1000)
            return (number / 1000f).ToString("0.#") + "K";
        return number.ToString();
    }
}
