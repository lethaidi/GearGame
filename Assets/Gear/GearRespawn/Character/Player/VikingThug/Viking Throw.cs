using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VikingThrow : MonoBehaviour
{
    public GameObject prefab;
    public Transform throwPoint;
    public Ally ally;
    public AudioSource audioSource;
    public bool hasThrown = false;

    void Update()
    {
        if (ally.isAttack && !hasThrown)
        {
            hasThrown = true;
            StartCoroutine(ThrowAxe());
        }
    }

    IEnumerator ThrowAxe()
    {
        yield return new WaitForSeconds(1f);

        Instantiate(prefab, throwPoint.position, throwPoint.rotation);
        hasThrown = false;
        if (audioSource != null)
        {
            audioSource.Play();
        }
        
    }
}
