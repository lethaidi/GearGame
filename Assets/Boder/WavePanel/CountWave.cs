using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountWave : MonoBehaviour
{
    [Header("Cài đặt hiển thị")]
    public TextMeshProUGUI text;
    public int showQuantityWave;
    public CountEnemy countEnemy;

    [Header("Cài đặt wave")]
    public int maxWave = 10;     // 🌟 Wave tối đa (tùy chỉnh trong Inspector)

    [Header("Cài đặt sau khi hoàn thành wave")]
    public bool isCompleteWave = false, isWinR10 = false, isChangeBG = false;
    public GameObject background, storeBoard, winBoard;
    public WaveManager waveManager;
    public ButtonStore buttonStore;
    public PrefabRespawn prefabRespawn;
    public RespawnEnemy respawnEnemy;
    public int temporaryValue = 0;

    public LevelManager levelManager;
    public bool isGuideWave = false;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        respawnEnemy = FindObjectOfType<RespawnEnemy>();
        waveManager = FindObjectOfType<WaveManager>();
        background = waveManager.background;
        winBoard.SetActive(false);
        classifyWave();
    }

    void Update()
    {
        if (levelManager.currentLevel == 3)
        {
            //win
        }

        // Kiểm tra hết enemy thì xử lý wave
        if (countEnemy.showQuantityEnemy == 0 && !isCompleteWave)
        {
            isCompleteWave = true;
            StartCoroutine(waitSound());
        }

        // Hiển thị số wave
        text.text = showQuantityWave.ToString() + "/" + maxWave;
    }

    IEnumerator waitSound()
    {
        yield return new WaitForSeconds(10f);

        // Xóa ally
        Ally[] allies = FindObjectsOfType<Ally>();
        foreach (Ally ally in allies)
            Destroy(ally.gameObject);

        showQuantityWave++;

        // Nếu vượt maxWave → Win
        if (showQuantityWave > maxWave)
        {
            showQuantityWave--;
            Win();
            yield break;
        }

        classifyWave();

        Vector3 startPos = background.transform.position;
        Vector3 endPos = new Vector3(startPos.x - 8f, startPos.y, startPos.z);

        // Di chuyển background
        yield return StartCoroutine(MoveBackground(startPos, endPos, 2f));

        if (respawnEnemy.isBossWave)
        {
            waveManager.isPassed = false;
            buttonStore.isPushFight = false;
            buttonStore.PushToRespawnEnemy = true;
            storeBoard.SetActive(false);
            StartCoroutine(AppearStoreBoard());
        }
        else
        {
            buttonStore.PushToRespawnEnemy = false;
            buttonStore.isPushFight = false;
            storeBoard.SetActive(true);
            waveManager.isPassed = true;
        }

        isCompleteWave = false;
        respawnEnemy.spawnedCount = 0;
    }

    IEnumerator AppearStoreBoard()
    {
        yield return new WaitForSeconds(3f);
        storeBoard.SetActive(true);
        waveManager.isPassed = true;
    }

    IEnumerator MoveBackground(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            background.transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        background.transform.position = to;

        bool isAdded = false;
        ColliderManager colliderManager = FindObjectOfType<ColliderManager>();
        if (colliderManager != null && !isAdded)
        {
            colliderManager.AddCount();
            colliderManager.isGuideActive = false;
            colliderManager.isturnColl = true;
            isAdded = true;
        }
    }

    void classifyWave()
    {
        // Phân loại wave
        if (showQuantityWave == 1)
        {
            respawnEnemy.isBossWave = false;
            prefabRespawn.currentWave = PrefabRespawn.WaveType.Wave1;
        }
        else
        {
            // Boss wave ở wave 5 và wave max
            if (showQuantityWave == 5 || showQuantityWave == maxWave)
            {
                respawnEnemy.isBossWave = true;
                prefabRespawn.currentWave = PrefabRespawn.WaveType.Between;
            }
            else
            {
                respawnEnemy.isBossWave = false;
                prefabRespawn.currentWave = PrefabRespawn.WaveType.Other;
            }
        }

        // Reset enemy count
        countEnemy.quantityEnemy = 0;

        if (prefabRespawn.currentWave == PrefabRespawn.WaveType.Wave1)
        {
            if (isGuideWave)
            {
                countEnemy.fixedQuantityEnemy = 1;
                temporaryValue = countEnemy.fixedQuantityEnemy;
            }
            else
            {
                countEnemy.fixedQuantityEnemy = 3;
                temporaryValue = countEnemy.fixedQuantityEnemy;
            }
        }
        else if (prefabRespawn.currentWave == PrefabRespawn.WaveType.Between)
        {
            countEnemy.fixedQuantityEnemy = 1;
        }
        else if (prefabRespawn.currentWave == PrefabRespawn.WaveType.Other)
        {
            if(isGuideWave)
            {
                temporaryValue += 0;
                countEnemy.fixedQuantityEnemy = temporaryValue;
            }
            else
            {
                temporaryValue += Random.Range(1, 3);
                countEnemy.fixedQuantityEnemy = temporaryValue;
            }
        }

        countEnemy.showQuantityEnemy = countEnemy.fixedQuantityEnemy;
        countEnemy.isPlayAudio = false;
    }

    void Win()
    {
        winBoard.SetActive(true);
        StartCoroutine(waitEndScene());
        Debug.Log("Bạn đã thắng!");
    }

    IEnumerator waitEndScene()
    {
        yield return new WaitForSeconds(0.3f);
        isWinR10 = true;
        isChangeBG = true;
    }
}
