using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStore : MonoBehaviour
{
    public int CountNear;
    //fight
    public GameObject Notice, Store, Blue, Grey;
    public bool isPushFight = false, PushToRespawnEnemy = false, isRoll = false;

    public Coin coin;
    void Start()
    {
        Grey.SetActive(false);
        Blue.SetActive(true);
        Store.SetActive(true);
        Notice.SetActive(false);
    }
    void Update()
    {
        if(coin.CoinValue < 5)
        {
            Grey.SetActive(true);
            Blue.SetActive(false);
        }
        else
        {
            Grey.SetActive(false);
            Blue.SetActive(true);
        }
        CheckNearGear();
    }
    void CheckNearGear()
    {
        // Tìm tất cả các GameObject có tag "Attacker"
        GameObject[] gears = GameObject.FindGameObjectsWithTag("Attacker");
        int newCount = 0;

        foreach (GameObject gearObj in gears)
        {
            float dist = Vector2.Distance(transform.position, gearObj.transform.position);
            if (dist > 2f)
            {
                newCount++;
            }
        }

        CountNear = newCount;
    }

    public void Fight()
    {
        if (CountNear == 0)
        {
            Notice.SetActive(true);
        }
        else
        {
            Store.SetActive(false);
            ControllGear[] oldGears = FindObjectsOfType<ControllGear>();
            foreach (ControllGear gear in oldGears)
            {
                float dist = Vector2.Distance(transform.position, gear.transform.position);
                if (dist < 2f)
                {
                    Destroy(gear.transform.parent.gameObject);
                }
            }
            isPushFight = true;
            PushToRespawnEnemy = true;
        }
    }
    public void ReRoll()
    {
        if(coin.CoinValue < 5)
        {
            return;
        }
        isRoll = true;
        ControllGear[] oldGears = FindObjectsOfType<ControllGear>();
        foreach (ControllGear gear in oldGears)
        {
            float dist = Vector2.Distance(transform.position, gear.transform.position);
            if (dist < 2f)
            {
                Destroy(gear.transform.parent.gameObject);
            }
        }
        coin.CoinValue -= 5;
        if (coin.CoinValue < 5)
        {
            Grey.SetActive(true);
            Blue.SetActive(false);
        }
    }
}
