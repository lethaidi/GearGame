using UnityEngine;
using UnityEngine.UI;
using TMPro;   // đúng namespace của TextMeshPro

public class CountCards : MonoBehaviour
{
    [Header("UI")]
    public Image targetImage;
    public TextMeshProUGUI countText;

    [Header("Data")]
    public ScriptableMiniGears gear;
    public int maxCount = 25;            // số tối đa (cho text và fill)

    private int lastGearCount = -1;

    void Start()
    {
        if (gear != null)
        {
            lastGearCount = gear.count;
            UpdateUI(lastGearCount);
        }
    }

    void Update()
    {
        if (gear == null || targetImage == null || countText == null || maxCount <= 0) return;

        if (gear.count != lastGearCount)
        {
            lastGearCount = gear.count;
            UpdateUI(lastGearCount);
        }
    }

    void UpdateUI(int count)
    {
        targetImage.fillAmount = Mathf.Clamp01((float)count / maxCount);
        countText.text = count.ToString() + "/" + maxCount;
    }
}
