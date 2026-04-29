using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWave : MonoBehaviour
{
    public GameObject guide;

    void Start()
    {
        if (guide == null)
        {
            guide = GameObject.Find("GuideOutSide");
            GuideOutSide Guide = guide.GetComponent<GuideOutSide>();
            Guide.EnableIndexGuide(Guide.lastcount - 10);
        }
    }
}
