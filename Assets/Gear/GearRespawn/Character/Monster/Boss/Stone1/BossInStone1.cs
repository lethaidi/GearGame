using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossInStone1 : MonoBehaviour
{
    public RespawnEnemy respawnEnemy;
    public CountEnemy countEnemy;
    public ButtonStore buttonStore;
    public HealthSystem healthSystem;
    public SpriteRenderer front;
    void Start()
    {
        front.sortingOrder = 2;
    }
    void Update()
    {
        if (buttonStore == null)
        {
            buttonStore = FindObjectOfType<ButtonStore>();
            buttonStore.PushToRespawnEnemy = false;
        }
        if (respawnEnemy == null)
        {
            respawnEnemy = FindObjectOfType<RespawnEnemy>();
            respawnEnemy.isBossWave = false;
        }

        if(countEnemy == null)
        {
            countEnemy = FindObjectOfType<CountEnemy>();
            countEnemy.showQuantityEnemy = 7;
            countEnemy.fixedQuantityEnemy = 7;
        }
        if(healthSystem.currentHealth <= 0)
        {
            buttonStore.PushToRespawnEnemy = false;
            countEnemy.showQuantityEnemy = 0;
        }
        if (buttonStore.PushToRespawnEnemy)
        {
            front.sortingOrder = 10;
        }
    }
}
