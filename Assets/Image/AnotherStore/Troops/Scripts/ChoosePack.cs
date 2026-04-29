using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ChoosePack : MonoBehaviour
{
    public BannerTroop bannerTroop;
    public List<GameObject> packChoices = new List<GameObject>();
    public AudioSource audioSource, audioSource1;

    [Header("State")]
    public int defaultChoice = 1;   // pack đang được trang bị
    public int currentChoice = -1;  // pack người chơi đang tạm chọn
    public bool isEquip = false;

    private const string SAVE_KEY = "DefaultPackChoiceName";

    void Awake()
    {
        // Lấy tên pack đã lưu
        string savedPackName = PlayerPrefs.GetString(SAVE_KEY, "");

        if (!string.IsNullOrEmpty(savedPackName))
        {
            for (int i = 0; i < packChoices.Count; i++)
            {
                if (packChoices[i] != null && packChoices[i].name == savedPackName)
                {
                    defaultChoice = i + 1;
                    break;
                }
            }
        }

        ApplyChoice(defaultChoice);
        isEquip = true;
    }

    void Update()
    {
        if (!isEquip && currentChoice != -1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Tạo raycast UI
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);

                bool clickedOnPack = false;

                // Duyệt tất cả các UI bị click
                foreach (var r in results)
                {
                    // Nếu click vào Pack hoặc nút Equip thì KHÔNG reset
                    if (r.gameObject.CompareTag("Pack") || r.gameObject.CompareTag("ButtonEquip"))
                    {
                        clickedOnPack = true;
                        break;
                    }
                }

                // Nếu không click vào pack hoặc nút equip => reset về pack đang equip
                if (!clickedOnPack)
                {
                    ApplyChoice(defaultChoice);
                    currentChoice = -1;
                    isEquip = true;
                }
            }
        }
    }

    // Khi click chọn pack
    public void Choose( int choice)
    {
        if (choice < 1 || choice > packChoices.Count)
            return;

        currentChoice = choice;

        // Ẩn tất cả pack
        foreach (GameObject pack in packChoices)
            if (pack != null) pack.SetActive(false);

        // Bật pack tạm chọn
        packChoices[choice - 1].SetActive(true);
        //Animate chọn pack
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject;
        if (clickedObj != null)
        {
            Animator animator = clickedObj.GetComponent<Animator>();
            if (animator != null)
                StartCoroutine(WaitAnimate(animator));
        }
        isEquip = false;

        if (bannerTroop != null)
        {
            bannerTroop.isPack1 = (choice == 1);
            bannerTroop.isPack2 = (choice == 2);
        }

        audioSource1.Play();
    }

    IEnumerator WaitAnimate(Animator animator)
    {
        animator.SetBool("IsChoose", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("IsChoose", false);
    }

    // Khi bấm nút Equip
    public void OnEquipButton()
    {
        if (currentChoice == -1)
            return; // chưa chọn pack nào

        defaultChoice = currentChoice;
        isEquip = true;

        // Lưu lại tên pack
        string packName = packChoices[defaultChoice - 1].name;
        PlayerPrefs.SetString(SAVE_KEY, packName);
        PlayerPrefs.Save();

        ApplyChoice(defaultChoice);

        audioSource.Play();
    }

    // Áp dụng pack được equip
    private void ApplyChoice(int choice)
    {
        if (choice < 1 || choice > packChoices.Count)
            return;

        foreach (GameObject pack in packChoices)
            if (pack != null) pack.SetActive(false);

        packChoices[choice - 1].SetActive(true);

        if (bannerTroop != null)
        {
            bannerTroop.isPack1 = (choice == 1);
            bannerTroop.isPack2 = (choice == 2);
        }
    }
}
