using System.Collections;
using UnityEngine;

public class Acorn : MonoBehaviour
{
    public ScriptableMiniGears supportGear;
    public shuriken shurikenScript;
    public GameObject targetObj, prefabEffect;

    private bool isBuffed = false;

    void Update()
    {
        if (targetObj == null && shurikenScript != null)
        {
            targetObj = shurikenScript.targetGaObj;
        }
    }

    void InCreaseSmash()
    {
        if (isBuffed || targetObj == null) return; // tránh buff nhiều lần
        isBuffed = true;

        // tăng tốc chạy
        Ally ally = targetObj.GetComponent<Ally>();
        if (ally != null) ally.moveSpeed += (1 + supportGear.count);

        // tăng damage
        Attack attackComp = targetObj.GetComponentInChildren<Attack>();
        if (attackComp != null) attackComp.DamageMax += (2 + supportGear.count);

        // spawn effect
        if (prefabEffect != null)
        {
            GameObject fx = Instantiate(prefabEffect, targetObj.transform.position, Quaternion.identity, targetObj.transform);
            Destroy(fx, 5f); // tự hủy sau 5s
        }

        StartCoroutine(BalanceSmash());
    }

    IEnumerator BalanceSmash()
    {
        yield return new WaitForSeconds(5f);

        if (targetObj != null)
        {
            Ally ally = targetObj.GetComponent<Ally>();
            if (ally != null) ally.moveSpeed -= (1 + supportGear.count);

            Attack attackComp = targetObj.GetComponentInChildren<Attack>();
            if (attackComp != null) attackComp.DamageMax -= (2 + supportGear.count);
        }

        isBuffed = false; // cho phép buff lại
        Destroy(this.gameObject); // hủy quả dẻ sau khi hết hiệu ứng
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == targetObj)
        {
            InCreaseSmash();
            this.gameObject.SetActive(false);
        }
    }
}
