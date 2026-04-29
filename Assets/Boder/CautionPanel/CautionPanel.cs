using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CautionPanel : MonoBehaviour
{
    public GameObject cautionPanel, Store;
    public ButtonStore buttonStore;

    public void Back()
    {
        cautionPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }
    public void Continue()
    {
        ControllGear[] oldGears = FindObjectsOfType<ControllGear>();
        foreach (ControllGear gear in oldGears)
        {
            float dist = Vector2.Distance(transform.position, gear.transform.position);
            if (dist < 3f && !gear.isBuy)
            {
                Destroy(gear.transform.parent.gameObject);
            }
        }
        buttonStore.PushToRespawnEnemy = true;
        buttonStore.isPushFight = true;
        cautionPanel.SetActive(false);
        Store.SetActive(false);
    }
    
}
