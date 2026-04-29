using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotGearStartScene : MonoBehaviour
{
    public SpriteRenderer gear, icon;
    public GearSlots slot;

    void Update()
    {
        if (slot != null )
        {
            gear.sprite = slot.gear.sprite;
            icon.sprite = slot.icon.sprite;
        }
    }
}
