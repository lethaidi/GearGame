using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeCanUpdate : MonoBehaviour
{
    public GameObject notice, icon;

    void Update()
    {
        if (FindObjectOfType() == 1)
        {
            notice.SetActive(true);
            Animator Icon = icon.GetComponent<Animator>();
            Icon.SetBool("canUpdate", true);
        }
        else
        {
            notice.SetActive(false);
            Animator Icon = icon.GetComponent<Animator>();
            Icon.SetBool("canUpdate", false);
        }
    }
    
    int FindObjectOfType ()
    {
        ButtonGears[] buttonGearsArray = FindObjectsOfType<ButtonGears>();
        foreach (ButtonGears buttonGears in buttonGearsArray)
        {
            if (buttonGears.canUpdate)
            {
                return 1;
            }
        }
        return 0;
    }
}
