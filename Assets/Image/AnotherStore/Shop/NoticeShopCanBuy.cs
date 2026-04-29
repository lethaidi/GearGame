using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeShopCanBuy : MonoBehaviour
{
    public GameObject flagNotice, icon;

    void Update()
    {
        if (FindObjectOfType() == 1)
        {
            flagNotice.SetActive(true);
            Animator Icon = icon.GetComponent<Animator>();
            Icon.SetBool("canUpdate", true);
        }
        else
        {
            flagNotice.SetActive(false);
            Animator Icon = icon.GetComponent<Animator>();
            Icon.SetBool("canUpdate", false);
        }
    }
    int FindObjectOfType()
    {
        BuyChest[] buttonShopArray = FindObjectsOfType<BuyChest>();
        foreach (BuyChest buttonShop in buttonShopArray)
        {
            if (buttonShop.canBuy)
            {
                return 1;
            }
        }
        return 0;
    }
}
