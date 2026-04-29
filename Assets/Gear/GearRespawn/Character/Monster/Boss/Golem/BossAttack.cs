using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float shootForce = 10f;
    public Boss ally;

    private bool isShooting = false;

    public AudioSource audioSource;

    void Update()
    {
        if (ally.isAttack && !isShooting)
        {
            StartCoroutine(waitShoot());
        }
    }

    IEnumerator waitShoot()
    {
        isShooting = true;

        ShootArrow(); // Bắn

        yield return new WaitForSeconds(1f);

        isShooting = false;
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Thiếu arrowPrefab hoặc firePoint.");
            return;
        }

        GameObject targetObj = ally.targetObj;
        if (targetObj == null)
        {
            Debug.LogWarning("Không có target từ ally.");
            return;
        }

        // Tính hướng từ firePoint đến mục tiêu
        Vector2 direction = (targetObj.transform.position - firePoint.position).normalized;

        // Tính góc quay dựa trên hướng
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Tạo mũi tên và xoay theo góc
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        // Gán vận tốc cho Rigidbody2D
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * shootForce;
        }
        // Phát âm thanh bắn mũi tên
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
