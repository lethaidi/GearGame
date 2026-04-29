using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject PauseBoard;

    void Start()
    {
        PauseBoard.SetActive(false);
    }

    public void PauseGame()
    {
        AudioListener.pause = true; // Pause all audio listeners
        Time.timeScale = 0f;
        PauseBoard.SetActive(true);
    }
    public void ResumeGame()
    {
        AudioListener.pause = false; // Resume all audio listeners 
        Time.timeScale = 1f;
        PauseBoard.SetActive(false);
    }
    public void ExitGame()
    {
        //Exit the game
        Application.Quit();
    }
}
