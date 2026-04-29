using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int count;
    private Queue<GameObject> pool = new Queue<GameObject>();
    public bool autoStart = true;

    public void StartPool()
    {
        if (!autoStart) return;

        for (int i = 0; i < count; i++)
        {
            // Tạo obj nhưng KHÔNG cần set parent ở đây để tránh méo scale
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(transform); // vẫn là con để dễ quản lý trong Hierarchy
            obj.transform.localPosition = Vector3.zero; // đặt vị trí về 0,0,0 so với parent
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        autoStart = false; // Chỉ khởi tạo một lần
    }

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);

            HealthSystem healthSystem = obj.GetComponent<HealthSystem>();
            Ally ally = obj.GetComponent<Ally>();
            if (healthSystem != null && ally != null)
            {
                healthSystem.objectPool = this;
                healthSystem.ResetState();
                ally.ResetState();
            }
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab);
            return obj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.position = this.transform.position;
        pool.Enqueue(obj);
    }

}
