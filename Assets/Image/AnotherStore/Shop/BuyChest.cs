using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyChest : MonoBehaviour
{
    [Header("Tham chiếu")]
    public GameObject prefab;           // Prefab rương muốn spawn
    public Transform spawnPoint;        // Vị trí spawn
    public GameObject targetcamera;     // Camera để đổi góc nhìn
    public GameObject head, bottom;
    public ChestOpener chestOpener;
    public Coin coin;
    public AudioSource audioSource;

    [Header("Nút mua")]
    public GameObject ButtonGreen;
    public GameObject ButtonGrey;

    [Header("Cấu hình")]
    public int count;                   // Số vật phẩm trong rương
    public int price;                   // Giá tiền mở rương
    public TaskSO taskOpenChest;        // Nhiệm vụ mở rương (nếu có)

    [Header("Trạng thái")]
    public bool canBuy = false;         // True khi đủ tiền

    void Update()
    {
        UpdateBuyState();
    }

    void UpdateBuyState()
    {
        // Kiểm tra có đủ tiền không
        canBuy = coin.CoinValue >= price;

        // Đổi màu nút
        ButtonGreen.SetActive(canBuy);
        ButtonGrey.SetActive(!canBuy);
    }

    public void SpawnObject()
    {
        // Chặn bấm nếu chưa đủ tiền
        if (!canBuy) return;

        // Cập nhật nhiệm vụ
        if (taskOpenChest != null && taskOpenChest.currentValue < taskOpenChest.targetValue)
        {
            taskOpenChest.currentValue++;
        }

        // Phát âm thanh
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Trừ xu
        StartCoroutine(MinusCoin());

        // Đổi góc camera
        targetcamera.transform.position = new Vector3(-55, 0, -10);

        // Spawn rương
        StartCoroutine(SpawnChest());
    }

    IEnumerator SpawnChest()
    {
        yield return new WaitForSeconds(1f);

        GameObject newObj = Instantiate(prefab, spawnPoint.position, Quaternion.identity, spawnPoint);

        // Giữ nguyên vị trí local
        newObj.transform.localPosition = Vector3.zero;
        newObj.transform.localRotation = Quaternion.identity;
        newObj.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(3f);

        // Sinh vật phẩm trong rương
        chestOpener.SpawnItems(count);
    }

    IEnumerator MinusCoin()
    {
        coin.CoinValue -= price;
        yield return new WaitForSeconds(0.1f);

        head.SetActive(false);
        bottom.SetActive(false);
    }
}
