using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeTroopUpdate : MonoBehaviour
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
        ButtonUpgrade[] buttonTroopsArray = FindObjectsOfType<ButtonUpgrade>();
        foreach (ButtonUpgrade buttonTroops in buttonTroopsArray)
        {
            if (buttonTroops.canUpgrade)
            {
                return 1;
            }
        }
        return 0;
    }
}
