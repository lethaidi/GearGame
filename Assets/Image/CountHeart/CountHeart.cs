using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountHeart : MonoBehaviour
{
    [Header("Cài đặt hiển thị")]
    public TextMeshProUGUI text;
    public int showQuantityHeart;
    public int initialHeart = 3; // <== chỉnh trong Inspector
    public bool isMinus = false;
    public GearMain gearMain;

    [Header("UI")]
    public GameObject DefeatPanel;

    [Header("Sound")]
    public AudioSource audioSource1;
    public bool isPlayAudio = false;

    private bool isGameOver = false;

    [Header("Skill")]
    public ScriptableSkills skillHealth;

    private int baseHeart;
    private int lastSkillHealth;

    void Start()
    {
        baseHeart = initialHeart;

        // Tính tổng tim ban đầu
        showQuantityHeart = baseHeart + skillHealth.valueSkill;

        lastSkillHealth = skillHealth.valueSkill;

        isGameOver = false;
        DefeatPanel.SetActive(false);

        text.text = showQuantityHeart.ToString();
    }

    void Update()
    {
        if (gearMain == null)
        {
            gearMain = FindObjectOfType<GearMain>();
        }

        UpdateShowQuantityHeart();
        text.text = showQuantityHeart.ToString();

        // Nếu skillHealth thay đổi -> cập nhật lại tim tối đa
        if (skillHealth.valueSkill != lastSkillHealth)
        {
            int diff = skillHealth.valueSkill - lastSkillHealth;

            // Cộng thêm tim vào hiện tại khi skill tăng
            showQuantityHeart += diff;

            // Cập nhật lại biến theo dõi
            lastSkillHealth = skillHealth.valueSkill;
        }

        if (showQuantityHeart == 0 && !isGameOver)
        {
            isGameOver = true;
            LostGame();
        }
    }

    void UpdateShowQuantityHeart()
    {
        if (isMinus)
        {
            showQuantityHeart--;
            isMinus = false;
        }

        if (showQuantityHeart < 0)
        {
            showQuantityHeart = 0;
        }
    }

    void LostGame()
    {
        DefeatPanel.SetActive(true);
        gearMain.isFight = false;
        Time.timeScale = 0f;

        if (!audioSource1.isPlaying && !isPlayAudio)
        {
            isPlayAudio = true;
            audioSource1.Play();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            isMinus = true;
        }
    }
}
