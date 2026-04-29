using UnityEngine;
using TMPro;

public class OtherTasks : MonoBehaviour
{
    [Header("Data")]
    public TaskSO Task;
    public Coin coin;

    [Header("UI")]
    public TextMeshProUGUI descriptText, rewardText;
    public GameObject buttonGreen;     // Claim
    public GameObject buttonGrey;      // Chưa đủ
    public GameObject buttonIsReceive; // Đã nhận

    private int lastValue = -1;

    void Start()
    {
        if (Task != null && descriptText != null)
            descriptText.text = Task.description;
        if (Task != null && rewardText != null)
            rewardText.text = "+" + Task.rewardCoins.ToString();

        lastValue = Task.currentValue;
        UpdateUI();
    }

    void Update()
    {
        if (Task == null) return;

        if (lastValue != Task.currentValue)
        {
            lastValue = Task.currentValue;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (Task.isCompleted && !Task.hasClaimedReward)
        {
            buttonGreen.SetActive(true);
            buttonGrey.SetActive(false);
            buttonIsReceive.SetActive(false);
        }
        else if (Task.hasClaimedReward)
        {
            buttonGreen.SetActive(false);
            buttonGrey.SetActive(false);
            buttonIsReceive.SetActive(true);
        }
        else
        {
            buttonGreen.SetActive(false);
            buttonGrey.SetActive(true);
            buttonIsReceive.SetActive(false);
        }
    }

    public void GiveReward()
    {
        if (coin != null && Task.isCompleted && !Task.hasClaimedReward)
        {
            // Thưởng coin
            coin.CoinValue += Task.rewardCoins;

            // Đánh dấu đã nhận
            Task.hasClaimedReward = true;

            UpdateUI();
        }
    }
}
