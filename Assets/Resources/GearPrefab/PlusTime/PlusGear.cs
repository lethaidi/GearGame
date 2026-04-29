using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlusGear : MonoBehaviour
{
    [Header("Plus Gear Settings")]
    public int level;
    public GameObject updateGear;
    public bool isReach = false;
    public bool isMerging = false, isMerged = false;
    public ControllGear controllGear;
    public GameObject num, gear;
    public PlusGear targetToMerge;

    public void MergeGear(PlusGear other)
    {
        if (other.level != level || isMerging ) return;
        if (!controllGear.isBuy || !other.controllGear.isBuy) return;
        isMerging = true;

        ControllGear otherCtrl = other.GetComponentInChildren<ControllGear>();

        Vector3 spawnPos = otherCtrl.tempSnap.position;

        // Tạo gear mới
        GameObject newGear = Instantiate(updateGear, spawnPos, Quaternion.identity);
        PlusGear newPlusGear = newGear.GetComponentInChildren<PlusGear>();
        newPlusGear.isMerged = true;
        // Lấy ControllGear mới trong prefab con
        ControllGear newCtrl = newGear.GetComponentInChildren<ControllGear>();
        if (newCtrl != null && otherCtrl != null)
        {
            newCtrl.coin = otherCtrl.coin;
            newCtrl.gearMain = otherCtrl.gearMain;
            newCtrl.isBuy = true;
            newCtrl.isSnap = true;
            newCtrl.isRotate = true;
            newCtrl.dragStore = true;
            newCtrl.but1 = otherCtrl.but1;
            newCtrl.but2 = otherCtrl.but2;
            newCtrl.tempSnap = otherCtrl.lastSnapZone.transform;
            newCtrl.lastSnapZone = otherCtrl.lastSnapZone;
            if(newCtrl.but1 == true)
            {
                newCtrl.Gear.transform.rotation = Quaternion.Euler(0, 0, 22.5f);
            }      
        }
        // Hủy 2 gear cũ (cha)
        StartCoroutine(WaitDestroy(other));
    }

    public void TryMergeIfPossible()
    {
        if (targetToMerge != null)
        {
            controllGear.lastSnapZone.SetActive(true);
            MergeGear(targetToMerge);
            targetToMerge = null;
        }
    }

    IEnumerator WaitDestroy(PlusGear other)
    {
        yield return new WaitForEndOfFrame();
        Destroy(other.transform.parent.gameObject);
        Destroy(transform.parent.gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        PlusGear plusGear = other.GetComponent<PlusGear>();
        if (plusGear != null && controllGear.isDragging && level == plusGear.level && controllGear.isBuy && plusGear.controllGear.isBuy)
        {
            num.GetComponent<SpriteRenderer>().color = new Color(0.61f, 0.61f, 0.61f, 1f);
            gear.GetComponent<SpriteRenderer>().color = new Color(0.61f, 0.61f, 0.61f, 1f);

            plusGear.num.GetComponent<SpriteRenderer>().color = new Color(0.61f, 0.61f, 0.61f, 1f);
            plusGear.gear.GetComponent<SpriteRenderer>().color = new Color(0.61f, 0.61f, 0.61f, 1f);

            isReach = true;
            targetToMerge = plusGear;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlusGear plusGear = other.GetComponent<PlusGear>();
        if (plusGear != null)
        {
            num.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            gear.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

            plusGear.num.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            plusGear.gear.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

            isReach = false;
            if (targetToMerge == plusGear)
                targetToMerge = null;
        }
    }

}
