using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shuriken : MonoBehaviour
{
    [Header("Shuriken Settings")]
    public float rotateSpeed = 500f;
    public float moveSpeed = 10f;
    public string targetTag;
    public GameObject targetGaObj = null;

    public float curveDuration = 0.8f;     // Thời gian bay theo đường cong
    public float curveHeight = 2f;         // Độ cao của đường cong

    [Header("Rotation Mode")]
    public bool isRotate = true;           // true = xoay vòng (shuriken), false = xoay hướng target (missile)

    private Transform target;
    private bool isCurving = true;         // Giai đoạn bay cong
    private Vector3 startPos;
    private Vector3 controlPoint;
    private Vector3 endPos;
    private float curveTime = 0f;

    public GearMain gearMain;
    public float rotationOffset = 0f;      // chỉnh trong Inspector

    void Start()
    {
        // Gán vị trí ban đầu
        startPos = transform.position;
        endPos = transform.position + new Vector3(0.5f, 1.5f, 0f);

        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObj != null)
        {
            targetGaObj = targetObj;
            target = targetObj.transform;
            StartCoroutine(WaitChangeEndPos());
        }

        // Tính điểm control (giữa start và end, nhưng nâng lên một chút tạo độ cong)
        Vector3 direction = (endPos - startPos).normalized;
        Vector3 adjustedEnd = endPos - direction * 0.2f;
        controlPoint = (startPos + adjustedEnd) / 2f + Vector3.up * curveHeight;
    }

    void Update()
    {
        if (gearMain == null)
        {
            gearMain = GameObject.FindObjectOfType<GearMain>();
        }
        if (!gearMain.isFight)
        {
            Destroy(gameObject);
        }

        // Xoay shuriken
        if (isRotate)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }

        // 1️⃣ Giai đoạn bay cong (Bezier)
        if (isCurving)
        {
            curveTime += Time.deltaTime / curveDuration;
            curveTime = Mathf.Clamp01(curveTime);

            // Phương trình Bezier bậc 2
            Vector3 m1 = Vector3.Lerp(startPos, controlPoint, curveTime);
            Vector3 m2 = Vector3.Lerp(controlPoint, endPos, curveTime);
            transform.position = Vector3.Lerp(m1, m2, curveTime);

            // Nếu muốn mũi tên xoay theo hướng bay:
            if (!isRotate)
            {
                Vector3 direction = (m2 - m1).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
            }

            if (curveTime >= 1f)
            {
                isCurving = false;
            }

            return;
        }

        // 2️⃣ Sau khi bay cong xong -> truy đuổi target
        if (target == null)
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
            if (targetObj != null)
            {
                target = targetObj.transform;
            }
        }

        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;

            if (!isRotate)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle + rotationOffset);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.deltaTime
                );
            }

            transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }

    IEnumerator WaitChangeEndPos()
    {
        yield return new WaitForSeconds(0.8f);
        endPos = target.position;
    }
}
