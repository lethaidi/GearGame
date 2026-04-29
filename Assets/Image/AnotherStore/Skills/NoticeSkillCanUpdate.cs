using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeSkillCanUpdate : MonoBehaviour
{
    public GameObject FlagNotice, icon;

    void Update()
    {
        if (FindObjectOfType() == 1)
        {
            FlagNotice.SetActive(true);
            Animator Icon = icon.GetComponent<Animator>();
            Icon.SetBool("canUpdate", true);
        }
        else
        {
            FlagNotice.SetActive(false);
            Animator Icon = icon.GetComponent<Animator>();
            Icon.SetBool("canUpdate", false);
        }
    }

    int FindObjectOfType()
    {
        BuySkills[] buySkillsArray = FindObjectsOfType<BuySkills>();
        foreach (BuySkills buySkills in buySkillsArray)
        {
            if (buySkills.canBuy)
            {
                return 1;
            }
        }
        return 0;
    }
}
