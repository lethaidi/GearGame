using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountEnemy : MonoBehaviour
{
    [Header("Count Enemy Settings")]
    public TextMeshProUGUI text;
    public int fixedQuantityEnemy;
    public int showQuantityEnemy;
    public int quantityEnemy = 0;
    List<GameObject> monster = new List<GameObject>();

    [Header("Another")]
    public ButtonStore buttonStore;
    public RespawnEnemy respawnEnemy;

    [Header("Audio")]
    public AudioSource audioSource;
    public bool isPlayAudio = false;

    [Header("Scriptable Task")]
    public TaskSO taskCountEnemy;

    private bool taskRewardGiven = false; // flag

    void Start()
    {
        showQuantityEnemy = fixedQuantityEnemy;
    }

    void Update()
    {
        CheckShowQuantityEnemy();

        // Lấy tất cả enemy theo tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            if (!monster.Contains(e))
            {
                monster.Add(e);
                quantityEnemy++;

                // ✅ có enemy mới spawn → reset flag
                taskRewardGiven = false;
            }
        }

        quantityEnemyDecrease();
        text.text = showQuantityEnemy.ToString();
    }

    void quantityEnemyDecrease()
    {
        for (int i = monster.Count - 1; i >= 0; i--)
        {
            if (monster[i] == null)
            {
                monster.RemoveAt(i);
                showQuantityEnemy--;
            }
        }
    }

    void CheckShowQuantityEnemy()
    {
        if (showQuantityEnemy == 0)
        {
            if (!audioSource.isPlaying && !isPlayAudio)
            {
                isPlayAudio = true;
                audioSource.Play();
            }

            buttonStore.isPushFight = false;
            buttonStore.PushToRespawnEnemy = false;

            if (respawnEnemy != null)
            {
                respawnEnemy.PausePlayer();
            }

            // ✅ chỉ cộng 1 lần cho mỗi đợt clear
            if (taskCountEnemy != null && !taskRewardGiven)
            {
                if (taskCountEnemy.currentValue < taskCountEnemy.targetValue)
                {
                    taskCountEnemy.currentValue++;
                }
                taskRewardGiven = true;
            }
        }
    }
}
