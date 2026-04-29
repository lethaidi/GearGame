using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BannerTroop : MonoBehaviour
{
    [Header("Packs")]
    public List<BannerPack> bannerPack = new List<BannerPack>();
    public List<GameObject> PackObjects = new List<GameObject>();
    public List<GameObject> CharObjects = new List<GameObject>();

    [Header("UI References")]
    public List<TextMeshProUGUI> Name = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> Rank = new List<TextMeshProUGUI>();

    [Header("Settings")]
    public bool isShowRank = false;
    public bool isPack1 = true;
    public bool isPack2 = false;

    private BannerPack currentPack; // Pack hiện tại đang hiển thị
    private int lastRank = -1;
    private string lastName = "";

    void Update()
    {
        ChangePack();
        if (currentPack == null || Name == null || Rank == null) return;

        // chỉ update khi có thay đổi
        if (currentPack.packRank != lastRank || currentPack.packName != lastName)
        {
            Show();
            lastRank = currentPack.packRank;
            lastName = currentPack.packName;
        }
    }

    void Show()
    {
        foreach (TextMeshProUGUI txt in Name)
            txt.text = currentPack.packName;
        foreach (TextMeshProUGUI rank in Rank)
        {
            if (isShowRank)
                rank.text = "Rank: " + currentPack.packRank;
            else
                rank.text = currentPack.packRank.ToString();
        }
    }

    void ChangePack()
    {
        if (bannerPack == null || bannerPack.Count == 0) return;

        if (isPack1 && bannerPack.Count > 0)
        {
            isPack1 = false;
            foreach (GameObject obj in PackObjects)
                obj.SetActive(false);
            foreach (GameObject charObj in CharObjects)
                charObj.SetActive(false);

            CharObjects[0].SetActive(true);
            PackObjects[0].SetActive(true);
            currentPack = bannerPack[0];
        }
        else if (isPack2 && bannerPack.Count > 1)
        {
            isPack2 = false;
            foreach (GameObject obj in PackObjects)
                obj.SetActive(false);
            foreach (GameObject charObj in CharObjects)
                charObj.SetActive(false);

            CharObjects[1].SetActive(true);
            PackObjects[1].SetActive(true);
            currentPack = bannerPack[1];
        }
    }
}
