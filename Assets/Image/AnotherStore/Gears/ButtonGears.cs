using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonGears : MonoBehaviour
{
    public ScriptableMiniGears miniGears;
    public TextMeshProUGUI statusText;
    public string searchTag;
    public Image image, imageRank;
    public Sprite green, grey;
    public int count;
    public GameObject theLock, lockGear, lockIcon, tickIcon;
    public bool isMax = false, isSelect = false, isChoosed = false, canUpdate = false;

    public AudioSource audioUnlock, audioUpgrade;
    public TaskSO taskUpgade;

    void Start()
    {
        if (miniGears != null)
        {
            count = miniGears.count;

            statusText.text = miniGears.savedText;
            imageRank.sprite = miniGears.savedRank != null ? miniGears.savedRank : miniGears.rankA;

            // Kiểm tra lock đã bị destroy chưa
            if (miniGears.isTheLockDestroyed && theLock != null)
                Destroy(theLock);
            if (miniGears.isLockGearDestroyed && lockGear != null)
                Destroy(lockGear);
            if (miniGears.isLockIconDestroyed && lockIcon != null)
                Destroy(lockIcon);
            UpdateButtonState();
        }
    }

    void Update()
    {
        // kiểm tra có thể unlock hoặc upgrade không
        if (count >= 25 && !isMax)
        {
            canUpdate = true;
        }
        else
        {
            canUpdate = false;
        }

        UpdateRankImage();
        if (isChoosed)
        {
            tickIcon.SetActive(true);
        }
        else
        {
            tickIcon.SetActive(false);
        }
        if (isSelect && Input.GetMouseButtonDown(0))
        {
            // kiểm tra object nào được click trong EventSystem
            GameObject clicked = EventSystem.current.currentSelectedGameObject;
            if (clicked == null || clicked != gameObject)
            {
                isSelect = false;
            }
        }
        if (isMax) return;
        AddCountByTag();
        UpdateButtonState();
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (miniGears != null && miniGears.isUnlocked)
        {
            isSelect = true;
        }
    }

    void AddCountByTag()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(searchTag);

        foreach (GameObject obj in objs)
        {
            if (obj.GetComponent<CountedMarker>() == null)
            {
                obj.AddComponent<CountedMarker>();
                count++;
                miniGears.count = count; // runtime only
                UpdateButtonState();
            }
        }
    }

    private void UpdateButtonState()
    {
        if (image != null)
        {
            image.sprite = (count >= 25) ? green : grey;
        }

        if (statusText != null)
        {
            if (miniGears.isUnlocked == false && count < 25)
                statusText.text = miniGears.locked;
            else if (miniGears.isUnlocked == false && count >= 25)
                statusText.text = miniGears.unlocked;
        }

        // Lưu trạng thái text và rank mỗi khi update
        miniGears.savedText = statusText.text;
        miniGears.savedRank = imageRank.sprite;
    }

    public void Unlock()
    {
        if (miniGears != null && count >= 25 && !miniGears.isUnlocked)
        {
            if (audioUnlock != null)
            {
                audioUnlock.Play();
            }
            miniGears.rank = 1;
            miniGears.isUnlocked = true;
            statusText.text = miniGears.upgrade;
            count -= 25;
            miniGears.count = count;
            imageRank.sprite = miniGears.rankA;
            if (theLock != null && lockGear != null)
            {
                miniGears.isTheLockDestroyed = true;
                miniGears.isLockGearDestroyed = true;
                miniGears.isLockIconDestroyed = true;

                Destroy(theLock);
                Destroy(lockGear);
                Destroy(lockIcon);
            }
        }
    }

    public void Upgrade()
    {
        if(isMax) return;
        if (miniGears != null && count >= 25 && miniGears.isUnlocked && !miniGears.isUpgrade)
        {
            if (taskUpgade != null && taskUpgade.currentValue < taskUpgade.targetValue)
            {
                taskUpgade.currentValue++;
            }
            if (audioUpgrade != null)
            {
                audioUpgrade.Play();
            }
            miniGears.isUpgrade = true;
            statusText.text = miniGears.upgrade;
            count -= 25;
            miniGears.count = count;

            if (miniGears.rank == 1)
            {
                miniGears.rank = 2;
                imageRank.sprite = miniGears.rankB;
            }
            else if (miniGears.rank == 2)
            {
                miniGears.rank = 3;
                imageRank.sprite = miniGears.rankS;
            }
            else if (miniGears.rank == 3)
            {
                isMax = true;
                statusText.text = "MAX RANK";
            }

            StartCoroutine(ResetUpgradeFlag());
        }
    }
    public void Select()
    {
        if (isChoosed) return;
        if (miniGears != null && miniGears.isUnlocked)
        {
            isSelect = true;
        }
    }
    IEnumerator ResetUpgradeFlag()
    {
        yield return new WaitForSeconds(1f);
        miniGears.isUpgrade = false;
    }
    void UpdateRankImage()
    {
        if (miniGears.rank == 1)
        {
            imageRank.sprite = miniGears.rankA;
        }
        else if (miniGears.rank == 2)
        {
            imageRank.sprite = miniGears.rankB;
        }
        else if (miniGears.rank == 3)
        {
            imageRank.sprite = miniGears.rankS;
            isMax = true;
            statusText.text = "MAX RANK";
        }
    }

}

/// <summary>
/// Script rỗng để đánh dấu object đã đếm
/// </summary>
public class CountedMarker : MonoBehaviour { }
