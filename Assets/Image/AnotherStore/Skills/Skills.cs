using UnityEngine;
using TMPro;

public class Skills : MonoBehaviour
{
    public ScriptableSkills scriptSkill;
    public TextMeshProUGUI value;
    private int lastValue = -1;

    void Start()
    {
        if (scriptSkill == null) return;

        // lấy key theo tên object (mỗi skill 1 key riêng)
        string key = gameObject.name + "_SkillValue";

        // nếu đã lưu trước đó thì load
        if (PlayerPrefs.HasKey(key))
        {
            scriptSkill.valueSkill = PlayerPrefs.GetInt(key);
            lastValue = scriptSkill.valueSkill;
        }

        Show();
    }

    void Update()
    {
        if (scriptSkill == null || value == null) return;

        if (scriptSkill.valueSkill != lastValue)
        {
            Show();
            lastValue = scriptSkill.valueSkill;

            // lưu lại khi có thay đổi
            string key = gameObject.name + "_SkillValue";
            PlayerPrefs.SetInt(key, lastValue);
            PlayerPrefs.Save();
        }
    }

    void Show()
    {
        value.text = scriptSkill.valueSkill.ToString();
    }
}
