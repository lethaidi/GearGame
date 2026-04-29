using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public float rotateSpeed;
    public float moveSpeed;
    public Vector3 direct;

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));
        transform.position += direct * moveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DelZone"))
        {
            Destroy(this.gameObject);
        }
    }
}
