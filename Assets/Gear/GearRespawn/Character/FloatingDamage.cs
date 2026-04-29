using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingDamage : MonoBehaviour
{
    [Header("Floating Damage Settings")]
    public float fadeOutSpeed;   // Tốc độ mờ dần
    public float lifetime; // Thời gian tồn tại trước khi biến mất

    private TextMeshProUGUI textMesh;
    private float timer;  

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        timer = 0f;
    }
    public void SetDamage(int damage)
    {
        if (textMesh != null)
        {
            textMesh.text = damage.ToString();
        }
        if (damage >= 40)
        {
            textMesh.color = Color.red;
            textMesh.fontSize = textMesh.fontSize * 1.2f;
        }
        else if (damage >= 100)
        {
            //màu tím
            textMesh.color = new Color(0.5f, 0f, 0.5f);
            textMesh.fontSize = textMesh.fontSize * 1.5f;
        }
    }
    void Update()
    {
        // Văng ra
        StartCoroutine(BezierMove(new Vector2(transform.position.x, transform.position.y + 0.5f), new Vector2(transform.position.x - 0.2f, transform.position.y), 0.5f));
        // Mờ dần theo thời gian
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
        Color color = textMesh.color;
        color.a = alpha;
        textMesh.color = color;
        // Hủy đối tượng sau khi hết thời gian tồn tại
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator BezierMove(Vector2 start, Vector2 end, float duration)
    {
        Vector2 control = new Vector2((start.x + end.x) / 2, Mathf.Max(start.y, end.y) + 1f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float u = 1 - t;
            Vector2 position = u * u * start + 2 * u * t * control + t * t * end;
            transform.position = position;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end; // đảm bảo kết thúc đúng vị trí
    }
}
