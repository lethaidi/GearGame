using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponsattack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackCooldown;
    public int DamageMin;
    public int DamageMax;
    private float lastAttackTime;
    public bool isAttack = false;

    public string targetTag;
    [Header("Effect when Destroy")]
    public GameObject destroyEffect;
    [Header("ScriptableObject")]
    public CharacterStats baseStats;
    public ScriptableMiniGears miniGears;
    public bool isDestroyed = true;
    void Start()
    {
        if (baseStats != null)
        {
            DamageMax = baseStats.attack;
        }
        if(miniGears != null)
        {
            if(miniGears.rank == 1) DamageMax += 3;
            else if (miniGears.rank == 2) DamageMax += 6;
            else if (miniGears.rank == 3) DamageMax += 10;
            else if (miniGears.rank == 0) DamageMax += 0;
        }
    }
    IEnumerator DestroyEndFrame()
    {
        if(!isDestroyed) yield break;

        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag) && !isAttack)
        {
            isAttack = true;
            GameObject target = other.gameObject;
            HealthSystem health = target.GetComponent<HealthSystem>();
            if (health != null)
            {
                int damage = Random.Range(DamageMin, DamageMax + 1);
                health.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
            // Destroy the weapon after the attack
            StartCoroutine(DestroyEndFrame());
        }
        if(other.CompareTag("DelZone"))
        {
            // Destroy the weapon if it hits the ground or platform
            StartCoroutine(DestroyEndFrame());
        }
    }
}
