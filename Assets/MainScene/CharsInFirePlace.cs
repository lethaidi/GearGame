using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharsInFirePlace : MonoBehaviour
{
    public SpriteRenderer char1, char2, char3;
    public Image char1Burned, char2Burned, char3Burned;

    void Update()
    {
        char1.sprite = char1Burned.sprite;
        char2.sprite = char2Burned.sprite;
        char3.sprite = char3Burned.sprite;
    }
}
