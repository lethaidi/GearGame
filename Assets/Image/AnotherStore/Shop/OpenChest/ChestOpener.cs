using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpener : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform itemContainer;      // Container chứa item UI
    public List<GameObject> itemPrefabs;     // Danh sách prefab item
    public Transform chestPos;               // Vị trí rương (UI hoặc world -> convert sang local)

    [Header("Grid Settings")]
    public int columns = 5;      // số cột
    public float spacingX = 150; // khoảng cách ngang
    public float spacingY = 200; // khoảng cách dọc

    [Header("Effect Settings")]
    public float spawnDelay = 0.2f;   // Delay giữa các item khi rơi ra
    public Vector2 randomRange = new Vector2(100, 100); // độ văng random

    private List<GameObject> spawnedItems = new List<GameObject>();
    [Header("Button Exit")]
    public GameObject buttonExit;

    /// <summary>
    /// Spawn nhiều item 1 lúc
    /// </summary>

    public void SpawnItems(int count)
    {
        StartCoroutine(SpawnMultiple(count));
    }

    private IEnumerator SpawnMultiple(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnSingleItem();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    /// <summary>
    /// Spawn 1 item (dùng nội bộ)
    /// </summary>
    private void SpawnSingleItem()
    {
        // Random prefab
        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

        GameObject newItem = Instantiate(prefab, itemContainer);
        RectTransform rect = newItem.GetComponent<RectTransform>();

        rect.localScale = Vector3.one;

        // Lấy vị trí chestPos quy đổi sang local của itemContainer
        // Lấy vị trí chestPos quy đổi sang local của itemContainer
        Vector3 chestLocalPos = itemContainer.InverseTransformPoint(chestPos.position);
        rect.localPosition = chestLocalPos;

        // Bay vòng nhỏ quanh rương
        float radius = 10f;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        Vector3 midTarget = chestLocalPos + offset;


        // Thêm vào danh sách
        spawnedItems.Add(newItem);

        // Slot cuối cùng trong list
        Vector3 targetPos = GetSlotPosition(spawnedItems.Count - 1);

        // Bay ra rồi gom lại
        StartCoroutine(MoveSequence(rect, midTarget, targetPos));
    }

    /// <summary>
    /// Tính vị trí slot trong grid
    /// </summary>
    private Vector3 GetSlotPosition(int index)
    {
        int row = index / columns;
        int col = index % columns;
        return new Vector3(col * spacingX, -row * spacingY, 0);
    }

    /// <summary>
    /// Coroutine move từ A -> B
    /// </summary>
    private IEnumerator MoveTo(RectTransform rect, Vector3 target, float duration, System.Action onComplete = null)
    {
        Vector3 start = rect.anchoredPosition;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            rect.anchoredPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }

        rect.anchoredPosition = target;
        onComplete?.Invoke();
    }

    /// <summary>
    /// Coroutine scale từ A -> B
    /// </summary>
    private IEnumerator ScaleTo(RectTransform rect, Vector3 target, float duration)
    {
        Vector3 start = rect.localScale;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            rect.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        rect.localScale = target;
    }
    /// <summary>
    /// Sequence: Chest -> Random -> Slot
    /// </summary>
    private IEnumerator MoveSequence(RectTransform rect, Vector3 midTarget, Vector3 finalTarget)
    {
        // Bước 1: chestPos -> random
        yield return MoveTo(rect, midTarget, 0.3f);

        // Scale nhỏ lại 1 chút khi gom
        yield return ScaleTo(rect, Vector3.one * 0.8f, 0.15f);

        // Bước 2: random -> slot
        yield return MoveTo(rect, finalTarget, 0.4f);

        // Scale về 1
        yield return ScaleTo(rect, Vector3.one, 0.15f);

        // Nếu là item cuối cùng trong list => show buttonExit
        if (spawnedItems.Count > 0 && rect.gameObject == spawnedItems[spawnedItems.Count - 1])
        {
            buttonExit.SetActive(true);
        }
    }


    /// <summary>
    /// Xóa item & sắp xếp lại list
    /// </summary>
    public void RemoveAllItems()
    {
        GameObject[] allChests = GameObject.FindGameObjectsWithTag("Chest");

        foreach (GameObject chest in allChests)
        {
            Destroy(chest);
        }

        foreach (var item in spawnedItems)
        {
            Destroy(item);
        }
        spawnedItems.Clear();
        buttonExit.SetActive(false);
    }

}
