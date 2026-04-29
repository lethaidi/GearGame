using UnityEngine;
using System;

public class TaskManager : MonoBehaviour
{
    public TaskUI[] allTaskUIs; // kéo tất cả TaskUI vào Inspector

    private static TaskManager instance;
    public static TaskManager Instance => instance;

    private const string LAST_LOGIN_KEY = "TM_LastLogin";
    private const string DAY_COUNTER_KEY = "TM_DayCounter";

    public int dayCounter = 0;
    public bool isResetting = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        CheckDayCounter();
    }
    void Update()
    {
        if (isResetting)
        {
            ResetAllTasks();
            isResetting = false;
        }
    }
    void CheckDayCounter()
    {
        string lastLogin = PlayerPrefs.GetString(LAST_LOGIN_KEY, "");
        DateTime today = DateTime.Now.Date;

        if (string.IsNullOrEmpty(lastLogin))
        {
            // lần đầu
            dayCounter = 1;
        }
        else
        {
            DateTime lastLoginDate = DateTime.Parse(lastLogin);

            if (lastLoginDate == today)
            {
                // đã login hôm nay → không tăng
            }
            else if (lastLoginDate == today.AddDays(-1))
            {
                // ngày kế tiếp → tăng
                dayCounter++;
            }
            else
            {
                // bỏ lỡ → reset về 1
                dayCounter = 1;
            }
        }

        // save lại
        PlayerPrefs.SetString(LAST_LOGIN_KEY, today.ToString());
        PlayerPrefs.SetInt(DAY_COUNTER_KEY, dayCounter);
        PlayerPrefs.Save();

        Debug.Log("📅 Ngày hiện tại trong chu kỳ: " + dayCounter);

        // Nếu qua 7 ngày → reset
        if (dayCounter > 7)
        {
            ResetAllTasks();
            dayCounter = 1;
            PlayerPrefs.SetInt(DAY_COUNTER_KEY, dayCounter);
            PlayerPrefs.Save();
        }
    }

    public void ResetAllTasks()
    {
        foreach (TaskUI taskUI in allTaskUIs)
        {
            taskUI.ResetUI();
        }
        Debug.Log("🔄 Sau 7 ngày: Toàn bộ Task đã reset!");
    }
}
