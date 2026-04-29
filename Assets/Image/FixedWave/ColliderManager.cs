using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    [Header("Danh sách object cần quản lý collider")]
    public List<GameObject> objectsToManage = new List<GameObject>();

    private List<BoxCollider2D> colliders = new List<BoxCollider2D>();

    public WaveManager waveManager;
    public List<GameObject> objectGuide = new List<GameObject>();
    public bool isGuideActive = false;
    public int lastcount = -1;
    public GearMain gearMain;
    public bool isEnableAll = true, isAdd = false, isturnColl = false;
    public SnapButon lastBut;
    private bool isChecked = false;

    void Awake()
    {
        // Lấy tất cả BoxCollider từ danh sách object
        foreach (GameObject obj in objectsToManage)
        {
            if (obj != null)
            {
                BoxCollider2D bc = obj.GetComponent<BoxCollider2D>();
                if (bc != null)
                    colliders.Add(bc);
                else
                    Debug.LogWarning(obj.name + " không có BoxCollider!");
            }
        }
        DisableAllColliders();
    }

    void Update()
    {
        if (gearMain == null)
        {
            gearMain = FindObjectOfType<GearMain>();
        }

        if (gearMain!=null && gearMain.isFight && !isAdd)
        {
            isAdd = true;
            AddCount();
            isGuideActive = false;
        }
        
        else if (gearMain != null && !gearMain.isFight && isAdd)
        {
            isAdd = false;
        }

        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
            lastcount = waveManager.isFirstPlay;
            Guide();
        }

        if (waveManager != null && lastcount != waveManager.isFirstPlay )
        {
            Debug.Log("Cập nhật hướng dẫn: " + waveManager.isFirstPlay);
            Guide();
        }

        if (waveManager.isFirstPlay == 8)
        {
            if (isChecked) return;
            PlusGear plusGear = FindObjectOfType<PlusGear>();
            if (plusGear != null && plusGear.isMerged && !isChecked)
            {
                isChecked = true;
                isGuideActive = false;
                waveManager.isFirstPlay++;
            }
        }
    }

    void Guide()
    {

        if (waveManager != null && !isGuideActive)
        {
            lastcount = waveManager.isFirstPlay;

            foreach (GameObject guide in objectGuide)
            {
                guide.SetActive(false);
            }
            DisableAllColliders();
            if (lastcount == 9)
            {
                EnableAllColliders();
            }
            if ((lastBut != null && lastBut.isEnableColl))
            {
                EnableAllColliders();
                lastBut.isEnableColl = false;
                isturnColl = false;
            }

            for (int i = 0; i < colliders.Count; i++)
            {
                if (i == lastcount)
                {
                    SetColliderActive(i, true);
                }
            }
            
            objectGuide[lastcount - 1].SetActive(true);

            isGuideActive = true;
        }
    }

    public void AddCount()
    {
        if (waveManager != null && waveManager.isFirstPlay < 8)
        {
            waveManager.isFirstPlay++;
        }
        
    }
    public void SpecialAddCount()
    {
        if (waveManager != null)
        {
            waveManager.isFirstPlay ++;
        }
    }
    /// <summary>
    /// Bật tất cả collider
    /// </summary>
    public void EnableAllColliders()
    {
        foreach (BoxCollider2D bc in colliders)
        {
            if (bc != null)
                bc.enabled = true;
        }
    }

    /// <summary>
    /// Tắt tất cả collider
    /// </summary>
    public void DisableAllColliders()
    {
        foreach (BoxCollider2D bc in colliders)
        {
            if (bc != null)
                bc.enabled = false;
        }
    }

    /// <summary>
    /// Bật/tắt collider theo index
    /// </summary>
    public void SetColliderActive(int index, bool active)
    {
        if (index >= 0 && index < colliders.Count)
        {
            colliders[index].enabled = active;
            
            SnapButon snapBut = colliders[index].GetComponent<SnapButon>();
            snapBut.isAdd = true;
            lastBut = snapBut;
        }
        else
        {
            Debug.LogWarning("Index không hợp lệ: " + index);
        }
    }

    /// <summary>
    /// Lấy collider theo index
    /// </summary>
    public BoxCollider2D GetCollider(int index)
    {
        if (index >= 0 && index < colliders.Count)
            return colliders[index];
        return null;
    }

    public void DisableAllCollidersInScene()
    {
        BoxCollider2D[] allColliders = FindObjectsOfType<BoxCollider2D>(true);

        foreach (var col in allColliders)
        {
            col.enabled = false;
        }

        Debug.Log("Đã tắt " + allColliders.Length + " colliders trong scene.");
    }

    public void EnableAllPhysicsColliders()
    {
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>(true);

        foreach (var col in allColliders)
        {
            col.enabled = true;
        }

        Debug.Log("Bật " + allColliders.Length + " colliders trong scene.");
    }

}
