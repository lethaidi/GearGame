using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceAttack : MonoBehaviour
{
    [Header("Thiết lập băng")]
    public GameObject IcePrefab; // Prefab băng
    public float attackRange = 5f; // Khoảng cách tấn công
    public int maxTargets = 3; // Số Ally tối đa bị đóng băng
    public float freezeDuration = 2f; // Thời gian đóng băng
    public float cooldown = 5f; // Thời gian hồi chiêu

    [Header("Thiết lập sát thương")]
    public int DamageMin = 5;
    public int DamageMax = 10;

    private bool isOnCooldown = false;

    void Update()
    {
        if (!isOnCooldown)
            TryAttack();
    }

    void TryAttack()
    {
        // Tìm tất cả Ally trong phạm vi
        GameObject[] allAllies = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> alliesInRange = new List<GameObject>();

        foreach (GameObject ally in allAllies)
        {
            float dist = Vector3.Distance(transform.position, ally.transform.position);

            // ally phải nằm trong phạm vi và bên trái enemy
            if (dist <= attackRange && ally.transform.position.x < transform.position.x)
            {
                alliesInRange.Add(ally);
            }
        }

        if (alliesInRange.Count == 0)
            return;

        // Chọn ngẫu nhiên Ally
        List<GameObject> chosenTargets = new List<GameObject>();
        List<GameObject> spawnedIces = new List<GameObject>();

        int count = Mathf.Min(maxTargets, alliesInRange.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, alliesInRange.Count);
            GameObject target = alliesInRange[randomIndex];
            alliesInRange.RemoveAt(randomIndex);

            if (target != null)
            {
                // Spawn băng
                GameObject ice = Instantiate(IcePrefab, target.transform.position, Quaternion.identity);
                spawnedIces.Add(ice);

                // Khóa di chuyển
                Ally allyScript = target.GetComponent<Ally>();
                if (allyScript != null)
                {
                    allyScript.canMove = false;
                    allyScript.isStop = true;
                    allyScript.isAttack = false;
                }

                // Gây damage nếu có Health
                HealthSystem health = target.GetComponent<HealthSystem>();
                if (health != null)
                {
                    int damage = Random.Range(DamageMin, DamageMax + 1);
                    health.TakeDamage(damage);
                }

                chosenTargets.Add(target);
            }
        }

        StartCoroutine(UnfreezeAfterDelay(chosenTargets, spawnedIces));
    }

    IEnumerator UnfreezeAfterDelay(List<GameObject> frozenTargets, List<GameObject> spawnedIces)
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(freezeDuration);

        foreach (GameObject target in frozenTargets)
        {
            if (target != null)
            {
                Ally allyScript = target.GetComponent<Ally>();
                if (allyScript != null)
                {
                    allyScript.isStop = false;
                    allyScript.canMove = true;
                    allyScript.isAttack = true;
                }
            }
        }

        foreach (GameObject ice in spawnedIces)
        {
            if (ice != null)
                Destroy(ice);
        }

        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}
