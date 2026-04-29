using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/Skill")]
public class ScriptableSkills : ScriptableObject
{
    public string skillName;
    public int valueSkill;

    [System.Serializable]
    private class SaveData
    {
        public string skillName;
        public int valueSkill;
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, name + ".json");

    public void Save()
    {
        var data = new SaveData() { skillName = skillName, valueSkill = valueSkill };
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    public void Load()
    {
        if (!File.Exists(SavePath)) return;
        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        skillName = data.skillName;
        valueSkill = data.valueSkill;
    }

    public void DeleteSave() { if (File.Exists(SavePath)) File.Delete(SavePath); }
}
