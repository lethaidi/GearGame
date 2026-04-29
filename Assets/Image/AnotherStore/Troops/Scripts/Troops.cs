using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Troops : MonoBehaviour
{
    public ScriptableSkills skillDama;
    public CharacterStats infor;

    public TextMeshProUGUI textAttack;
    public TextMeshProUGUI textHealth;
    public TextMeshProUGUI textLevel;

    public int upgradeHealth;
    public int upgradeAttack;

    private int baseHealth;
    private int baseAttack;

    private int bonusHealth = 0;
    private int bonusAttack = 0;

    private int lastSkillDama;

    public bool isUpgrade = false;

    void Start()
    {
        baseAttack = infor.attack;
        baseHealth = infor.maxHealth;

        lastSkillDama = skillDama.valueSkill;

        RecalculateStats();
        Show();
    }

    void Update()
    {
        // Khi nhấn upgrade
        if (isUpgrade)
        {
            bonusHealth += upgradeHealth;
            bonusAttack += upgradeAttack;
            infor.level += 1;

            RecalculateStats();
            Show();

            isUpgrade = false;
        }

        // Kiểm tra thay đổi skill
        if (skillDama.valueSkill != lastSkillDama)
        {
            RecalculateStats();
            Show();

            lastSkillDama = skillDama.valueSkill;
        }
    }

    void RecalculateStats()
    {
        infor.attack = baseAttack + bonusAttack + skillDama.valueSkill;
        infor.maxHealth = baseHealth + bonusHealth ;
    }

    void Show()
    {
        if (infor != null)
        {
            textHealth.text = infor.maxHealth.ToString();
            textAttack.text = infor.attack.ToString() + "/s";
            textLevel.text = infor.level.ToString();
        }
    }
}
