using System.Collections;
using UnityEngine;

public class ArcherShoot : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject arrowPrefab;      // Prefab mũi tên
    public Transform firePoint;         // Vị trí bắn mũi tên
    public float arcHeight;      // Độ cao của đường bay (vòng cung)
    public float flightDuration; // Thời gian bay
    public Ally ally;
    public AudioSource shootSound;
    public bool isShooting = false;

    [Header("Target")]
    public Transform target, temp;            // Mục tiêu bắn

    void Update()
    {
        if (target == null && ally.target != null)
        {
            target = ally.target.transform;
            temp = target;
            isShooting = false;
        }

        if (ally.isAttack && !isShooting)
        {
            StartCoroutine(waitShoot());
        }       
    }

    IEnumerator waitShoot()
    {
        isShooting = true;

        yield return new WaitForSeconds(0.5f);

        ShootArrow(target.position);
        isShooting = false;
    }
    // Hàm bắn mũi tên
    public void ShootArrow(Vector2 targetPos)
    {
        if (arrowPrefab == null || firePoint == null)
            return;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        OnArrowHit(arrow);
    
        if (shootSound != null)
        {
            shootSound.Play();
        }
        StartCoroutine(ArrowArc(arrow, firePoint.position, targetPos, flightDuration));
    }

    // Tạo hiệu ứng bay cong theo đường Bezier bậc 2
    IEnumerator ArrowArc(GameObject arrow, Vector2 start, Vector2 end, float duration)
    {
        // Điểm điều khiển ở giữa, nâng cao để tạo parabol
        Vector2 direction = (end - start).normalized;
        Vector2 adjustedEnd = end - direction * 0.2f;

        Vector2 control = (start + adjustedEnd) / 2 + Vector2.up * arcHeight;

        float t = 0f;
        Vector2 prevPos = start;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // Tính vị trí theo đường Bezier
            Vector2 pos = Mathf.Pow(1 - t, 2) * start
                        + 2 * (1 - t) * t * control
                        + Mathf.Pow(t, 2) * end;

            arrow.transform.position = pos;

            // Tính hướng bay hiện tại để xoay mũi tên
            Vector2 dir = (pos - prevPos).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // Xoay mượt theo hướng bay
            Quaternion targetRot = Quaternion.Euler(0, 0, angle);
            arrow.transform.rotation = Quaternion.Lerp(arrow.transform.rotation, targetRot, 0.3f);

            prevPos = pos;

            yield return null;
        }

    }

    // Hiệu ứng khi mũi tên chạm đích
    void OnArrowHit(GameObject arrow)
    {
        Destroy(arrow, flightDuration);
    }
}
