using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Danh sách các màn (theo thứ tự)")]
    public GameObject[] levels;

    [Header("Danh sách nhân vật (ScriptableObject)")]
    public List<CharacterStats> characters;

    public int currentLevel = -1;          // Màn lớn hiện tại
    public int currentSubRound = 1;        // Vòng nhỏ hiện tại
    public int maxSubRound = 5;            // Tổng số vòng nhỏ trong 1 màn
    public int lastCurrentLevel = -1;

    [Header("Thông tin wave")]
    public TextMeshProUGUI waveProgress;
    public Image slideProgess;

    void Start()
    {
        lastCurrentLevel = currentLevel;
        int savedLevel = PlayerPrefs.GetInt("SavedLevel", 0);
        int savedSubRound = PlayerPrefs.GetInt("SavedSubRound", 1);

        currentSubRound = savedSubRound;
        LoadLevel(savedLevel);

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }
    public void LoadLevel(int levelIndex)
    {
        // Xóa các màn trước
        for (int i = 0; i < levelIndex; i++)
        {
            if (levels[i] != null)
            {
                Destroy(levels[i]);
            }
        }

        // Tắt các màn sau
        for (int i = levelIndex + 1; i < levels.Length; i++)
        {
            if (levels[i] != null)
            {
                levels[i].SetActive(false);
            }
        }

        // Bật màn hiện tại
        if (levels[levelIndex] != null)
        {
            levels[levelIndex].SetActive(true);
            currentLevel = levelIndex;

            PlayerPrefs.SetInt("SavedLevel", currentLevel);
            PlayerPrefs.SetInt("SavedSubRound", currentSubRound);
            PlayerPrefs.Save();

            Debug.Log($"Đang ở màn {currentLevel + 1}, vòng nhỏ {currentSubRound}/{maxSubRound}");
        }
    }

    public void NextLevel()
    {
        // Tăng vòng nhỏ
        currentSubRound++;

        if (currentSubRound > maxSubRound)
        {
            // Qua màn mới
            currentSubRound = 1;
            int nextLevel = currentLevel + 1;

            if (nextLevel < levels.Length)
            {
                LoadLevel(nextLevel);

                // 👉 Khi qua màn mới thì tăng chỉ số cho tất cả nhân vật
                foreach (var character in characters)
                {
                    character.IncreaseStats(200, 30); // HP +100, ATK +50
                }
            }
            else
            {
                Debug.Log("Hết màn!");
            }
        }
        else
        {
            // Vẫn trong cùng màn, chỉ đổi vòng nhỏ
            Debug.Log($"Qua vòng nhỏ {currentSubRound}/{maxSubRound} trong màn {currentLevel + 1}");
        }

        PlayerPrefs.SetInt("SavedLevel", currentLevel);
        PlayerPrefs.SetInt("SavedSubRound", currentSubRound);
        PlayerPrefs.Save();
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("SavedLevel");
        PlayerPrefs.DeleteKey("SavedSubRound");

        currentSubRound = 1;
        LoadLevel(0);
    }

    void UpdateUI()
    {
        waveProgress.text = $"Wave: {currentSubRound}/{maxSubRound}";
        slideProgess.fillAmount = (float)currentSubRound / maxSubRound;
    }
}
