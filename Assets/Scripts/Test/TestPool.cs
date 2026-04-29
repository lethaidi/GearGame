using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPool : MonoBehaviour
{
    public ObjectPool bullet;
    public float shootForce = 500f;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject obj = bullet.GetObject();
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero; // Reset velocity
        rb.AddForce(transform.forward * shootForce);

        StartCoroutine(Deactive(obj, 2f)); // Deactivate after 2 seconds
    }

    IEnumerator Deactive(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        bullet.ReturnObject(obj);
    }

}
