using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;   // đúng namespace của TextMeshPro

public class ProgressUI : MonoBehaviour
{
    [Header("UI")]
    public Image targetImage;
    public TextMeshProUGUI countText;

    [Header("Data")]
    public TaskSO task;
    public int maxCount;            // số tối đa (cho text và fill)

    private int lastGearCount = -1;

    void Start()
    {
        if (task != null)
        {
            maxCount = task.targetValue;
            lastGearCount = task.currentValue;
            UpdateUI(lastGearCount);
        }
    }

    void Update()
    {
        if (task == null || targetImage == null || countText == null || maxCount <= 0) return;

        if (task.currentValue != lastGearCount)
        {
            lastGearCount = task.currentValue;
            UpdateUI(lastGearCount);
        }
    }

    void UpdateUI(int count)
    {
        targetImage.fillAmount = Mathf.Clamp01((float)count / maxCount);
        countText.text = count.ToString() + "/" + maxCount;
    }
}
