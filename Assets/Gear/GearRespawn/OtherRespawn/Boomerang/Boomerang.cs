using UnityEngine;
using System.Collections;

public class Boomerang : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 10f;
    public float rotateSpeed = 500f;   // tốc độ xoay tròn
    public float searchRadius = 10f;   // bán kính tìm kẻ địch
    public float delayBeforeReturn = 0.5f; // thời gian delay sau khi trúng enemy lần 1

    private Transform target;
    private Vector3 moveDir;
    private Vector3 spawnPoint;        // vị trí spawn ban đầu

    private int hitCount = 0;
    private bool returning = false;
    private bool isLaunched = false;

    void Start()
    {
        spawnPoint = transform.position;   // lưu lại vị trí spawn
        FindTargetAndLaunch();
    }

    void Update()
    {
        if (!isLaunched) return;

        // Xoay tròn quanh trục Z
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

        if (!returning)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
        else
        {
            // quay về vị trí spawn ban đầu
            Vector3 dir = (spawnPoint - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, spawnPoint) < 0.2f)
            {
                Destroy(gameObject);
            }
        }
    }

    void FindTargetAndLaunch()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (Collider2D col in hits)
        {
            if (col.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = col.transform;
                }
            }
        }

        if (nearest != null)
        {
            target = nearest;
            Launch(target.position);
        }
    }

    public void Launch(Vector3 targetPos)
    {
        moveDir = (targetPos - transform.position).normalized;
        isLaunched = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            hitCount++;

            if (hitCount == 1)
            {
                // Lần 1 → tiếp tục bay, sau đó mới quay về
                StartCoroutine(DelayReturn());
            }
            else if (hitCount >= 2)
            {
                // Lần 2 → biến mất luôn
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DelayReturn()
    {
        yield return new WaitForSeconds(delayBeforeReturn);
        returning = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
