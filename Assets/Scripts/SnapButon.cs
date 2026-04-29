using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapButon : MonoBehaviour
{
    public GameObject But1, But2;
    public List<ControllGear> dragGear = new List<ControllGear>();

    private bool isBut1Active = true; // để tránh gọi SetActive nhiều lần không cần thiết

    public ColliderManager colliderManager;
    public bool isSnapped = false, isAdd = false, isEnableColl = false;

    void Start() 
    {
        But1.SetActive(true);
        But2.SetActive(false);
        isBut1Active = true;
    }

    void Update()
    {
        if (colliderManager == null)
        {
            colliderManager = FindObjectOfType<ColliderManager>();
        }
        CheckedGearIsDragging();
        if (isSnapped && colliderManager != null && isAdd)
        {
            if (colliderManager != null)
            {
                colliderManager.isGuideActive = false;
                colliderManager.AddCount();
            }
            isSnapped = false;
            isAdd = false;
        }
    }

    void CheckedGearIsDragging()
    {
        ControllGear[] dragGear = FindObjectsOfType<ControllGear>();
        bool anyDragging = false;

        foreach (var obj in dragGear)
        {
            if (obj != null && obj.isDragging)
            {
                anyDragging = true;
                break;
            }
        }

        if (anyDragging && isBut1Active)
        {
            But1.SetActive(false);
            But2.SetActive(true);
            isBut1Active = false;
        }
        else if (!anyDragging && !isBut1Active)
        {
            But1.SetActive(true);
            But2.SetActive(false);
            isBut1Active = true;
        }
    }

}
