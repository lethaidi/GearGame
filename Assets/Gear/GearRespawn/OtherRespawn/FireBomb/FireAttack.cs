using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack : MonoBehaviour
{
    // Lưu lại coroutine theo từng enemy để dễ quản lý
    private Dictionary<Collider2D, Coroutine> activeCoroutines = new Dictionary<Collider2D, Coroutine>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Nếu enemy chưa có coroutine → bắt đầu gây damage
            if (!activeCoroutines.ContainsKey(other))
            {
                Coroutine c = StartCoroutine(DamageOverTime(other));
                activeCoroutines.Add(other, c);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Nếu enemy rời khỏi vùng → dừng gây damage
            if (activeCoroutines.ContainsKey(other))
            {
                StopCoroutine(activeCoroutines[other]);
                activeCoroutines.Remove(other);
            }
        }
    }

    IEnumerator DamageOverTime(Collider2D other)
    {
        HealthSystem health = other.GetComponent<HealthSystem>();

        while (other != null && health != null)
        {
            health.isUp = true;
            health.TakeDamage(Random.Range(1,2));

            // Gây damage mỗi giây
            yield return new WaitForSeconds(1f);
        }

        // Khi enemy bị destroy hoặc null → xóa khỏi danh sách
        if (activeCoroutines.ContainsKey(other))
            activeCoroutines.Remove(other);
    }
}
