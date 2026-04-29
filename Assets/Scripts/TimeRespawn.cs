using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeRespawn : MonoBehaviour
{
    public TextMeshProUGUI text;
    public ControllGear controllGear;

    public GearRespawn gearRespawn;
    public float ValueStart = 0f;

    public bool isStart = false;

    void Update()
    {
        if (text != null && !isStart)
        {
            text.text = ValueStart.ToString("F2") + "/s";
            isStart = true;
        }
        else if (controllGear.isSnap)
        {
            text.text = gearRespawn.conditionValue.ToString("F2") + "/s";
        }
    }
}
