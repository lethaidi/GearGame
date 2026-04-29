using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour
{
    public GameObject targetCamera;
    public int positionX;
    public GameObject Head, Bottom;
    public ChestOpener chestOpener;
    public bool isHideHeaderBottom = false;  

    // nhớ public nếu bạn muốn gọi hàm này từ Button OnClick
    public void TeleToFixedPosition()
    {
        if (targetCamera != null)
        {
            targetCamera.transform.position = new Vector3(
                positionX,
                targetCamera.transform.position.y, // viết thường
                targetCamera.transform.position.z  // viết thường
            );
            targetCamera.transform.rotation = Quaternion.Euler(0, 0, 0); // reset rotation
        }
        if(Head != null && Bottom != null && !isHideHeaderBottom)
        {
            Head.SetActive(true);
            Bottom.SetActive(true);
        }
        else if (Head != null && Bottom != null && isHideHeaderBottom)
        {
            Head.SetActive(false);
            Bottom.SetActive(false);
        }
        else if(Head != null && isHideHeaderBottom)
        {
            Head.SetActive(false);
        }
        else if (Bottom != null && isHideHeaderBottom)
        {
            Bottom.SetActive(false);
        }
        if (chestOpener != null)
        {
            chestOpener.RemoveAllItems();
        }
    }
}
