using System.Collections;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    public AudioSource audioSource;
    public float timedestroy = 1;
    void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    IEnumerator WaitAndDestroy()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        else
        {
            yield return new WaitForSeconds(timedestroy); // mặc định nếu không có audio
        }

        Destroy(gameObject); // chỉ phá chính object này
    }
}
