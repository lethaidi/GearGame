using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComingSoon : MonoBehaviour
{
    public void LoadGame()
    {
        // Xoá hết dữ liệu đã lưu (coin, level, setting,...)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Load lại scene 0 (mặc định scene chính)
        SceneManager.LoadScene(0);
    }
}
