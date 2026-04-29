using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRespawn : MonoBehaviour
{
    public GameObject objectToSpawn;         // Prefab muốn spawn
    public Transform[] spawnPoints;          // Các vị trí cho trước

    void Start()
    {
        if (objectToSpawn == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Thiếu prefab hoặc spawn points.");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform randomPoint = spawnPoints[randomIndex];

        Instantiate(objectToSpawn, randomPoint.position, randomPoint.rotation);

        GearMain gearMain = FindObjectOfType<GearMain>();
        if (randomIndex >= 0 && randomIndex <= 11)
        {
            gearMain.isSn1 = true;
        }
        else
        {
            gearMain.isSn1 = false;
        }
    }
}

