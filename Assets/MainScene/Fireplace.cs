using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireplace : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        StartCoroutine(WaitPlaySound());
    }

    IEnumerator WaitPlaySound()
    {
        yield return new WaitForSeconds(4.5f);
        audioSource.Play();
    }
}
