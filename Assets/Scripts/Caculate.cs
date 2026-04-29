using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculate : MonoBehaviour
{
    public ControllGear targetGear;
    private bool previousIsReachHead = false;

    void Update()
    {
        if (targetGear == null) return;

        // Chỉ tính toán khi isReachHead vừa chuyển từ false → true
        if (targetGear.isReachHead && !previousIsReachHead)
        {
            float total = targetGear.CalculateRecursive(new HashSet<ControllGear>(), targetGear);
            Debug.Log("Total value: " + total);
        }

        previousIsReachHead = targetGear.isReachHead;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Main"))
        {
            targetGear.isReach = true;
            targetGear.isReachHead = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Main"))
        {
            targetGear.isReach = false;
            targetGear.isReachHead = false;
        }
    }
}
