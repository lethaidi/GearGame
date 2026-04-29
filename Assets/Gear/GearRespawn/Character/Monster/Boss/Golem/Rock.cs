using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float speed = 5f; // Tốc độ di chuyển của đá
    public Vector2 moveDirection = new Vector2(-1, 0); // Hướng di chuyển mặc định
    public float speedRotation = 100f;

    public AudioSource audioSource;

    void Start()
    {
        moveDirection.Normalize();
        audioSource.Play();
    }
    void Update()
    {
        // Di chuyển đá theo hướng đã chỉ định
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // Xoay đá quanh trục Z
        transform.Rotate(0f, 0f, speedRotation * Time.deltaTime);
    }
}
