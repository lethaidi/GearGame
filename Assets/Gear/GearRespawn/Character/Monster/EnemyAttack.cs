using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int DamageMin;
    public int DamageMax;

    public Monster ally;

    public AudioSource audioSource;
    public string targetTag;
    [Header("ScriptableObject")]
    public CharacterStats baseStats;
    void Start()
    {
        if (baseStats != null)
        {
            DamageMax = baseStats.attack;
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            GameObject target = other.gameObject;
            HealthSystem health = target.GetComponent<HealthSystem>();
            if (health != null)
            {
                int damage = Random.Range(DamageMin, DamageMax + 1);
                health.TakeDamage(damage);
            }
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}
