using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainGuide : MonoBehaviour
{
    public bool isAdd = false, needGuide = false;
    public GuideOutSide guide;

    public void Add()
    {
        if (guide != null && !isAdd && needGuide)
        {
            guide.AddCount();
            guide.isGuideActive = false;
            needGuide = false;
            isAdd = true;
        }
    }

}
