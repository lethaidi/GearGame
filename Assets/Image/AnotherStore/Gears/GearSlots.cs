using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GearSlots : MonoBehaviour
{
    [Header("UI & Effect")]
    public Animator animator;
    public GameObject arrow;
    public Image gear, icon;
    public Sprite baseGear, baseIcon;

    [Header("Runtime State")]
    public GameObject targetObj = null;
    public GameObject lastTarget = null;

    [Header("Data Save")]
    public ScriptableSlotGear spGear;

    public AudioSource audioSource;

    void Start()
    {
        if (arrow != null) arrow.SetActive(false);

        // Khôi phục trạng thái từ ScriptableObject
        LoadFromScriptable();
    }

    void Update()
    {
        CheckAnyGearIsSelect();

        // Click ra ngoài => bỏ chọn
        if (targetObj != null && Input.GetMouseButtonDown(0))
        {
            GameObject clicked = EventSystem.current.currentSelectedGameObject;
            if (clicked == null || clicked != gameObject)
            {
                targetObj = null;
            }
        }
    }

    void CheckAnyGearIsSelect()
    {
        ButtonGears[] gears = FindObjectsOfType<ButtonGears>();
        bool anySelect = false;

        foreach (var obj in gears)
        {
            if (obj != null && obj.isSelect)
            {
                targetObj = obj.gameObject;
                anySelect = true;
                break;
            }
        }

        if (targetObj == null)
        {
            arrow.SetActive(false);
            animator.SetBool("isFlicker", false);
            return;
        }

        ButtonGears buttonGear = targetObj.GetComponent<ButtonGears>();
        if (buttonGear == null || buttonGear.isSelect == false)
        {
            arrow.SetActive(false);
            animator.SetBool("isFlicker", false);
            return;
        }

        arrow.SetActive(anySelect);
        animator.SetBool("isFlicker", anySelect);
    }

    public void Choose()
    {
        // Nếu đang trang bị mà click bỏ chọn => unequip
        if (spGear.isEquipped && lastTarget != null && targetObj == null)
        {
            ButtonGears lastButton = lastTarget.GetComponent<ButtonGears>();
            if (lastButton != null)
            {
                lastButton.isChoosed = false;
            }

            spGear.isEquipped = false;
            spGear.Slot = null;

            gear.sprite = baseGear;
            icon.sprite = baseIcon;

            lastTarget = null;
            targetObj = null;
            return;
        }

        // Trang bị gear mới
        if (targetObj != null)
        {
            ButtonGears buttonGear = targetObj.GetComponent<ButtonGears>();
            if (buttonGear != null && buttonGear.miniGears != null)
            {
                if (buttonGear.isChoosed)
                {
                    // Gear này đã được chọn rồi => bỏ qua
                    targetObj = null;
                    return;
                }

                gear.sprite = buttonGear.miniGears.gear;
                icon.sprite = buttonGear.miniGears.icon;
                buttonGear.isChoosed = true;

                // Cập nhật ScriptableObject
                if (audioSource != null)
                {
                    audioSource.Play();
                }
                spGear.isEquipped = true;
                spGear.Slot = buttonGear.miniGears.prefabGear;
            }
        }

        // Reset gear cũ
        if (lastTarget != null && lastTarget != targetObj)
        {
            ButtonGears lastButton = lastTarget.GetComponent<ButtonGears>();
            if (lastButton != null)
                lastButton.isChoosed = false;
        }

        lastTarget = targetObj;
        targetObj = null;
    }

    void LoadFromScriptable()
    {
        if (spGear != null && spGear.isEquipped && spGear.Slot != null)
        {
            // Tìm lại button gear tương ứng
            ButtonGears[] gears = FindObjectsOfType<ButtonGears>();
            foreach (var g in gears)
            {
                if (g.miniGears != null && g.miniGears.prefabGear == spGear.Slot)
                {
                    gear.sprite = g.miniGears.gear;
                    icon.sprite = g.miniGears.icon;
                    g.isChoosed = true;
                    lastTarget = g.gameObject;
                    break;
                }
            }
        }
        else
        {
            gear.sprite = baseGear;
            icon.sprite = baseIcon;
            lastTarget = null;
        }
    }
}
