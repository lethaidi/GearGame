using System;
using UnityEngine;
using TMPro;

public class DailyLoginManager : MonoBehaviour
{
    public TaskSO dailyStreakTask;
    public TextMeshProUGUI streakText, rewardText;
    private const string LAST_LOGIN_KEY = "LastLoginDate";
    public int lastValuae = -1;
    public Coin coin;
    public GameObject buttonGreen, buttonGrey, buttonIsReceive;

    void Start()
    {
        CheckDailyLogin();
        rewardText.text = "+" + dailyStreakTask.rewardCoins.ToString();
        lastValuae = dailyStreakTask.currentValue;
    }

    void Update()
    {
        if (lastValuae != dailyStreakTask.currentValue)
        {
            lastValuae = dailyStreakTask.currentValue;
            CheckDailyLogin();
        }
    }

    void CheckDailyLogin()
    {
        string lastLogin = PlayerPrefs.GetString(LAST_LOGIN_KEY, "");
        DateTime today = DateTime.Now.Date;

        if (string.IsNullOrEmpty(lastLogin))
        {
            // Lần đầu đăng nhập
            dailyStreakTask.currentValue = 1;
            dailyStreakTask.hasClaimedReward = false;
        }
        else
        {
            DateTime lastLoginDate = DateTime.Parse(lastLogin);

            if (lastLoginDate == today)
            {
                // Đã đăng nhập hôm nay → không tăng
            }
            else if (lastLoginDate == today.AddDays(-1))
            {
                // Ngày kế tiếp → tăng streak
                if (dailyStreakTask.hasClaimedReward)
                {
                    // Nếu hôm qua đã nhận thưởng → reset vòng mới
                    dailyStreakTask.ResetProgress();
                    dailyStreakTask.currentValue = 1;
                }
                else
                {
                    dailyStreakTask.currentValue++;
                }
            }
            else
            {
                // Bỏ lỡ → reset streak
                dailyStreakTask.ResetProgress();
                dailyStreakTask.currentValue = 1;
            }
        }

        // Lưu ngày đăng nhập
        PlayerPrefs.SetString(LAST_LOGIN_KEY, today.ToString());
        PlayerPrefs.Save();

        // Cập nhật UI
        streakText.text = dailyStreakTask.description;

        // Nút hiển thị
        if (dailyStreakTask.isCompleted && !dailyStreakTask.hasClaimedReward)
        {
            buttonGreen.SetActive(true);
            buttonGrey.SetActive(false);
        }
        else
        {
            buttonGreen.SetActive(false);
            buttonGrey.SetActive(true);
        }
    }

    public void GiveReward()
    {
        if (coin != null && dailyStreakTask.isCompleted && !dailyStreakTask.hasClaimedReward)
        {
            buttonIsReceive.SetActive(true);
            buttonGreen.SetActive(false);

            // Thưởng
            coin.CoinValue += dailyStreakTask.rewardCoins;

            // Đánh dấu đã nhận
            dailyStreakTask.hasClaimedReward = true;
        }
    }
}
