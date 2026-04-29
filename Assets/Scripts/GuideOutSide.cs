using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideOutSide : MonoBehaviour
{
    public WaveManager waveManager;
    public List<GameObject> listGuide = new List<GameObject>();
    public List<ContainGuide> listContainGuide = new List<ContainGuide>();
    public int lastcount = -1;
    public bool isGuideActive = false;

    void Update()
    {
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
            lastcount = waveManager.isFirstPlay;
        }

        if (waveManager != null && waveManager.isFirstPlay >= 10 && lastcount != waveManager.isFirstPlay && !isGuideActive)
        {
            lastcount = waveManager.isFirstPlay;
            isGuideActive = true;
            Guide();
        }
    }

    void Guide()
    {
        DisableAllGuide();
        EnableIndexGuide(lastcount - 10);
    }

    public void DisableAllGuide()
    {
        foreach (GameObject obj in listGuide)
        {
            obj.SetActive(false);
        }
    }

    public void EnableIndexGuide(int index)
    {
        listGuide[index].SetActive(true);
        listContainGuide[index].needGuide = true;
    }

    public void AddCount()
    {
        waveManager.isFirstPlay++;
    }
}
