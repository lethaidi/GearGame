using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class AutoButton : MonoBehaviour
{
    public string targetScriptName; // Tên class script (vd: "DefeatManager")
    public string targetFunction;   // Tên hàm cần gọi (vd: "Replay")

    void Start()
    {
        Button btn = GetComponent<Button>() ?? GetComponentInChildren<Button>();
        if (btn == null)
        {
            Debug.LogWarning("Không tìm thấy Button!");
            return;
        }

        // Tìm tất cả script trong scene
        MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
        MonoBehaviour target = null;

        // Kiểm tra tên class trùng với targetScriptName
        foreach (var s in scripts)
        {
            if (s.GetType().Name == targetScriptName)
            {
                target = s;
                break;
            }
        }

        if (target != null)
        {
            MethodInfo method = target.GetType().GetMethod(
                targetFunction,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (method != null)
            {
                btn.onClick.AddListener(() => method.Invoke(target, null));
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy hàm '{targetFunction}' trong script '{targetScriptName}'");
            }
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy script '{targetScriptName}' trong scene");
        }
    }
}
