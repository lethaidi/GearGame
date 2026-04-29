using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ExperienceBar : MonoBehaviour
{
    [Header("UI")]
    public Image expSlider; // Image kiểu Filled (Fill Method: Horizontal)

    [Header("EXP Info")]
    public int targetExp = 0;
    public List<BannerPack> packs;
    public BannerPack currentPack;
    public ChoosePack choosePack;
    private int selectedPackIndex = 0;

    [Header("Animation")]
    public float expSpeed = 100f; // tốc độ tăng EXP mỗi giây

    void Start()
    {
        selectedPackIndex = choosePack.defaultChoice - 1;
        currentPack = packs[selectedPackIndex];

        targetExp = currentPack.currentExp;
        UpdateUI();
    }

    void Update()
    {
        // Cập nhật pack nếu đổi pack
        int newIndex = choosePack.defaultChoice - 1;
        if (selectedPackIndex != newIndex)
        {
            selectedPackIndex = newIndex;
            currentPack = packs[selectedPackIndex];
            targetExp = currentPack.currentExp;
            UpdateUI();
        }
        
        // Animate EXP từ current → target
        if (currentPack.currentExp < targetExp)
        {
            currentPack.currentExp += Mathf.CeilToInt(expSpeed * Time.deltaTime);

            // Nếu vượt EXP tối đa → level up
            while (currentPack.currentExp >= currentPack.expToNextLevel)
            {
                currentPack.currentExp -= currentPack.expToNextLevel;
                targetExp -= currentPack.expToNextLevel;
                LevelUp();
            }

            UpdateUI();
        }
    }

    // Gọi khi thêm EXP
    public void AddExp(int amount)
    {
        targetExp += amount;
    }

    // Level up
    private void LevelUp()
    {
        currentPack.packRank += 1;
        currentPack.expToNextLevel = Mathf.RoundToInt(currentPack.expToNextLevel * 1.2f);
        UpdateUI();
    }

    // Cập nhật UI Image
    private void UpdateUI()
    {
        if (expSlider != null && currentPack.expToNextLevel > 0)
        {
            float fill = (float)currentPack.currentExp / currentPack.expToNextLevel;
            expSlider.fillAmount = Mathf.Clamp01(fill);
        }
    }
}
