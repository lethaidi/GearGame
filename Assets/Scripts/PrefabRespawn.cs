using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GearWithChance
{
    public GameObject prefab;
    [Range(0f, 100f)] public float spawnChance;
}
[System.Serializable]
public class Prefab
{
    public GameObject prefab;
}

[System.Serializable]
public class GearSpawnCharacter
{
    public Prefab[] prefabs = new Prefab[3];
}

public class PrefabRespawn : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GearMain gearMain;

    [Header("Matrix Gears")]
    public GearSpawnCharacter[] matrixGears;
    public ChoosePack choosePack;
    private bool isUpdate = false;

    [Header("Wave 1")]
    public GearWithChance[] wave1Gears;
    [Header("Wave Between")]
    public GearWithChance[] waveBetweenGears;
    [Header("Other Waves")]
    public GearWithChance[] otherWaveGears;

    [Header("Dynamic Slot Gears")]
    public ScriptableSlotGear spGear1;
    public ScriptableSlotGear spGear2;
    public ScriptableSlotGear spGear3;
    public bool isAdd = true;

    public enum WaveType { Wave1, Between, Other }
    public WaveType currentWave = WaveType.Wave1;

    public WaveManager waveManager;
    public Coin coin;
    public ButtonStore buttonStore;

    void Update()
    {
        if (choosePack == null)
        {
            choosePack = FindObjectOfType<ChoosePack>();
        }

        if (choosePack != null && !isUpdate)
        {
            // thêm gear từ matrix vào danh sách spawn wave1Gears và otherWaveGears
            int packIndex = choosePack.defaultChoice - 1;
            if (packIndex >= 0 && packIndex < matrixGears.Length)
            {
                GearSpawnCharacter selectedPack = matrixGears[packIndex];
                List<GearWithChance> wave1List = new List<GearWithChance>(wave1Gears);
                List<GearWithChance> otherList = new List<GearWithChance>(otherWaveGears);
                foreach (var prefabEntry in selectedPack.prefabs)
                {
                    if (prefabEntry.prefab != null)
                    {
                        wave1List.Add(new GearWithChance { prefab = prefabEntry.prefab, spawnChance = 10f });
                        otherList.Add(new GearWithChance { prefab = prefabEntry.prefab, spawnChance = 5f });
                    }
                }
                wave1Gears = wave1List.ToArray();
                otherWaveGears = otherList.ToArray();
            }
            isUpdate = true;
        }

        if (coin.CoinValue < 5 && !buttonStore.isRoll)
        {
            return; // Không spawn nếu không có tiền
        }

        waveManager = FindObjectOfType<WaveManager>();
        gearMain = FindObjectOfType<GearMain>();

        if (waveManager.isPassed)
        {
            SpawnRandomGears();
            waveManager.isPassed = false;
            buttonStore.isRoll = false;
        }
    }

    public void SpawnRandomGears()
    {
        GearWithChance[] selectedWave = GetWaveGearList();
        List<Transform> availableSpawns = new List<Transform>(spawnPoints);

        if (currentWave == WaveType.Wave1)
        {
            // Spawn mỗi prefab 1 lần
            List<GameObject> guaranteedPrefabs = new List<GameObject>();
            foreach (var gear in selectedWave)
            {
                if (gear.prefab != null)
                    guaranteedPrefabs.Add(gear.prefab);
            }

            ShuffleList(guaranteedPrefabs);

            foreach (var prefab in guaranteedPrefabs)
            {
                if (availableSpawns.Count == 0) break;

                int index = Random.Range(0, availableSpawns.Count);
                Transform spawnPoint = availableSpawns[index];
                availableSpawns.RemoveAt(index);

                SpawnGearAt(prefab, spawnPoint);
            }

            while (availableSpawns.Count > 0)
            {
                GameObject prefab = GetRandomGearByChance(selectedWave);
                if (prefab == null) break;

                int index = Random.Range(0, availableSpawns.Count);
                Transform spawnPoint = availableSpawns[index];
                availableSpawns.RemoveAt(index);

                SpawnGearAt(prefab, spawnPoint);
            }
        }
        else
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                GameObject selectedPrefab = GetRandomGearByChance(selectedWave);
                if (selectedPrefab == null) continue;

                SpawnGearAt(selectedPrefab, spawnPoint);
            }
        }
    }

    void SpawnGearAt(GameObject prefab, Transform spawnPoint)
    {
        GameObject newGear = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        foreach (ControllGear gearScript in newGear.GetComponentsInChildren<ControllGear>(true))
        {
            gearScript.gearMain = gearMain;

            Transform tp = newGear.transform.Find("TelePoint");
            if (tp != null)
                gearScript.telePoint = tp.gameObject;
        }

        foreach (GearRespawn respawnScript in newGear.GetComponentsInChildren<GearRespawn>(true))
        {
            respawnScript.gearMain = gearMain;
        }
    }

    GearWithChance[] GetWaveGearList()
    {
        switch (currentWave)
        {
            case WaveType.Wave1:
                return wave1Gears;

            case WaveType.Between:
                {
                    List<GearWithChance> tempList = new List<GearWithChance>(waveBetweenGears);

                    if (spGear1 != null && spGear1.Slot != null)
                        tempList.Add(new GearWithChance { prefab = spGear1.Slot, spawnChance = 100f });

                    if (spGear2 != null && spGear2.Slot != null)
                        tempList.Add(new GearWithChance { prefab = spGear2.Slot, spawnChance = 100f });

                    if (spGear3 != null && spGear3.Slot != null)
                        tempList.Add(new GearWithChance { prefab = spGear3.Slot, spawnChance = 100f });

                    return tempList.ToArray();
                }

            case WaveType.Other:
            default:
                {
                    List<GearWithChance> tempList = new List<GearWithChance>(otherWaveGears);

                    if (spGear1 != null && spGear1.Slot != null && isAdd)
                        tempList.Add(new GearWithChance { prefab = spGear1.Slot, spawnChance = 10f });

                    if (spGear2 != null && spGear2.Slot != null && isAdd)
                        tempList.Add(new GearWithChance { prefab = spGear2.Slot, spawnChance = 10f });

                    if (spGear3 != null && spGear3.Slot != null && isAdd)
                        tempList.Add(new GearWithChance { prefab = spGear3.Slot, spawnChance = 10f });

                    return tempList.ToArray();
                }
        }
    }

    GameObject GetRandomGearByChance(GearWithChance[] gears)
    {
        float totalChance = 0f;
        foreach (var gear in gears) totalChance += gear.spawnChance;

        float rand = Random.Range(0f, totalChance);
        float currentSum = 0f;

        foreach (var gear in gears)
        {
            currentSum += gear.spawnChance;
            if (rand <= currentSum)
                return gear.prefab;
        }
        return null;
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }
}
