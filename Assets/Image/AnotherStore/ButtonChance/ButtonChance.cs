using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChance : MonoBehaviour
{
    public AudioSource audioSource; // Gắn AudioSource vào đây để phát âm thanh 

    [Header("Danh sách các nút")]
    public List<Animator> buttonAnimators; // gắn 5 Animator của 5 nút vào đây

    void Start()
    {
        buttonAnimators[0].SetBool("isSelect", true); // Mặc định chọn nút đầu tiên
    }
    public void OnButtonSelected(int index)
    {
        // Tắt hết
        for (int i = 0; i < buttonAnimators.Count; i++)
        {
            buttonAnimators[i].SetBool("isSelect", false);
        }

        // Bật cái được chọn
        buttonAnimators[index].SetBool("isSelect", true);
        // Phát âm thanh
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
    public void TurnOffAllButtons()
    {
        for (int i = 0; i < buttonAnimators.Count; i++)
        {
            buttonAnimators[i].SetBool("isSelect", false);
        }
    }
}

