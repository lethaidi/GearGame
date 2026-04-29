using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float currentValue = 0f;
    public float conditionValue;

    public ControllGear controllGear;
    public GearMain gearMain;

    public float lastMainValue = -1f;
    public bool isCalculate = false;

    public int maxSpawn;
    public bool isSpawning = false, isSet = false;

    public AudioSource audioSource;

    [Header("ObjectPool")]
    public ObjectPool objectPool;
    public GameObject prefab;
    [Header("UI Settings")]
    public Image image, image2;
    public bool isFullFill = false;
    public CharacterStats characterStats;
    public SpriteRenderer sprite1;
    public Sprite sprite2;
    public GameObject prefab2;
    public GameObject img1, img2;
    void Update()
    {
        gearMain = FindObjectOfType<GearMain>();

        if (controllGear.isSnap)
        {
            float calculatedValue = controllGear.total;
            if (calculatedValue != lastMainValue)
            {
                conditionValue = calculatedValue;
                lastMainValue = calculatedValue;
            }
        }

        if (!controllGear.isReach)
        {
            isCalculate = false;
        }

        OnGearReached();
        FillImage();
        if (isSet == false)
        {
            isSet = true;
            if (characterStats != null)
                ChangeLevel();
            if(objectPool != null)
            {
                objectPool.StartPool();
            }
        }
        
    }

    void OnGearReached()
    {
        if (controllGear.isSnap && controllGear.isReach && !isCalculate && gearMain.isFight && !isFullFill)
        {
            isCalculate = true;
            currentValue += conditionValue;
        }
        else if(gearMain.isFight == false)
        {
            currentValue = 0f;
        }
        CalculateToRespawn();
    }

    void CalculateToRespawn()
    {
        if (currentValue >= 1f && !isSpawning)
        {
            isSpawning = true;
            isFullFill = true;
            currentValue = 0;
            StartCoroutine(SpawnWithDelay());
            StartCoroutine(Wait());
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        isSpawning = false;
        isCalculate = false; // Reset để chuẩn bị đợt tiếp theo
    }
    IEnumerator SpawnWithDelay()
    {

        for (int i = 0; i < maxSpawn; i++)
        {
            if(audioSource != null)
            {
                audioSource.Play();
            }
            Debug.Log("Spawm");
            SpawnObject();

            yield return new WaitForSeconds(0.2f);            
        }
    }

    void SpawnObject()
    {
        if(objectPool != null)
        {
            GameObject obj = objectPool.GetObject();
        } 
        else if (prefab != null)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(this.transform); // vẫn là con để dễ quản lý trong Hierarchy
            obj.transform.localPosition = Vector3.zero; // đặt vị trí về 0,0,0 so với parent
        }
    }

    void FillImage()
    {
        if (image != null)
        {
            // chạy cho mượt
            float targetfill = 1 - currentValue;
            image.fillAmount = Mathf.Lerp(image.fillAmount, targetfill, Time.deltaTime * 5f);
        }
        if (isFullFill)
        {
            StartCoroutine(WaitFill());
        }
    }

    IEnumerator WaitFill()
    {
        yield return new WaitForSeconds(1.5f);
        isFullFill = false;
    }

    void ChangeLevel()
    {
        if(characterStats.level >= 10)
        {
            sprite1.sprite = sprite2;
            objectPool.prefab = prefab2;
            image = image2;
            img1.SetActive(false);
            img2.SetActive(true);
        }
    }
}
