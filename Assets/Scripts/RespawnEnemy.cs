using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy
{
    public GameObject prefab;
}
[System.Serializable]
public class MatrixEnemy
{
    public Enemy[] enemies = new Enemy[3];
}

public class RespawnEnemy : MonoBehaviour
{
    public MatrixEnemy[] matrixEnemies;
    public LevelManager levelManager;

    [Header("Các Prefab Có Thể Respawn")]
    public GameObject[] objectsToSpawn; // Prefab enemy thường
    public GameObject[] bossPrefabs;    // Prefab Boss
    public GameObject[] bossPrefabs1;    // Prefab Boss

    [Header("Vị Trí Respawn")]
    public Transform[] spawnPoints;     // Các điểm spawn enemy thường
    public Transform spawnBoss, spawnBoss1;         // Điểm spawn boss

    [Header("Biến đếm để Respawn")]
    private List<Ally> countedAllies = new List<Ally>();

    public CountEnemy countEnemy;
    public bool isSpawning = false, isBossWave = false, isGuide = false;
    public ButtonStore buttonStore;
    public int spawnedCount = 0; // số lượng enemy đã spawn

    void Start()
    {
        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        // 🔹 Đảm bảo matrixEnemies hợp lệ
        if (matrixEnemies == null || matrixEnemies.Length == 0)
            return;

        // 🔹 Xác định index theo level (nếu level vượt quá thì lấy max)
        int indexToAdd = Mathf.Min(levelManager.currentLevel, matrixEnemies.Length - 1);

        // 🔹 Gộp enemy từ matrixEnemies[indexToAdd] vào objectsToSpawn
        if (matrixEnemies[indexToAdd] != null)
        {
            List<GameObject> newEnemies = new List<GameObject>(objectsToSpawn);

            foreach (var enemy in matrixEnemies[indexToAdd].enemies)
            {
                if (enemy != null && enemy.prefab != null)
                {
                    newEnemies.Add(enemy.prefab);
                }
            }

            objectsToSpawn = newEnemies.ToArray();
            Debug.Log($"Đã thêm {matrixEnemies[indexToAdd].enemies.Length} enemy từ Matrix[{indexToAdd}] vào danh sách spawn (tổng: {objectsToSpawn.Length}).");
        }
    }


    void Update()
    {
        if (!buttonStore.PushToRespawnEnemy || spawnedCount >= countEnemy.fixedQuantityEnemy)
            return;

        if(levelManager == null)
        {
            levelManager = FindObjectOfType<LevelManager>();
        }

        if (!isBossWave)
        {
            int allyCount = FindObjectsOfType<Ally>().Length;

            if (allyCount > 0)
                CountPlayer();
            else
                CheckIfNoPlayer();
        }
        else
        {
            SpawnBoss();
        }
    }

    void CountPlayer()
    {
        Ally[] allies = FindObjectsOfType<Ally>();
        foreach (Ally ally in allies)
        {
            if (ally.activated && !countedAllies.Contains(ally))
            {
                if (spawnedCount < countEnemy.fixedQuantityEnemy)
                    SpawnEnemy();

                countedAllies.Add(ally);
            }
        }
    }

    void CheckIfNoPlayer()
    {
        if (isGuide)
            return;
        if (FindObjectsOfType<Ally>().Length == 0 && !isSpawning)
            StartCoroutine(EnemyAfterDelay(8f));
    }

    IEnumerator EnemyAfterDelay(float delay)
    {
        isSpawning = true;
        yield return new WaitForSeconds(delay);

        if (FindObjectsOfType<Ally>().Length == 0 && spawnedCount < countEnemy.fixedQuantityEnemy)
            SpawnEnemy();

        isSpawning = false;
    }

    void SpawnBoss()
    {
        if ((bossPrefabs.Length == 0 && bossPrefabs1.Length == 0) ||
            (spawnBoss == null && spawnBoss1 == null))
            return;

        if (spawnedCount >= countEnemy.fixedQuantityEnemy)
            return;

        // Random chọn 0 = bossPrefabs, 1 = bossPrefabs1
        int bossType = Random.Range(0, 2);

        if (bossType == 0 && bossPrefabs.Length > 0 && spawnBoss != null)
        {
            int randomIndex = Random.Range(0, bossPrefabs.Length);
            GameObject prefab = bossPrefabs[randomIndex];
            Instantiate(prefab, spawnBoss.position, spawnBoss.rotation);
            spawnedCount++;
        }
        else if (bossType == 1 && bossPrefabs1.Length > 0 && spawnBoss1 != null)
        {
            int randomIndex = Random.Range(0, bossPrefabs1.Length);
            GameObject prefab = bossPrefabs1[randomIndex];
            Instantiate(prefab, spawnBoss1.position, spawnBoss1.rotation);
            spawnedCount++;
        }
    }


    void SpawnEnemy()
    {
        if (objectsToSpawn.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Thiếu prefab hoặc spawn points.");
            return;
        }

        if (spawnedCount >= countEnemy.fixedQuantityEnemy) return;

        int randomPrefabIndex = Random.Range(0, objectsToSpawn.Length);
        int randomPointIndex = Random.Range(0, spawnPoints.Length);

        GameObject prefab = objectsToSpawn[randomPrefabIndex];
        Transform spawnPoint = spawnPoints[randomPointIndex];

        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        spawnedCount++;
    }

    public void PausePlayer()
    {
        Ally[] allies = FindObjectsOfType<Ally>();
        foreach (Ally ally in allies)
        {
            if (ally.activated)
                ally.isWin = true;
        }
    }
}
