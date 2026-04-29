using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Game/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string characterName;

    [Header("Chỉ số")]
    public int maxHealth = 100;
    public int attack = 20;
    public int level = 1;

    public void IncreaseStats(int healthIncrease, int attackIncrease)
    {
        maxHealth += healthIncrease;
        attack += attackIncrease;
        level++;
    }

    [System.Serializable]
    private class SaveData
    {
        public string characterName;
        public int maxHealth, attack, level;
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, name + ".json");

    public void Save()
    {
        var data = new SaveData() { characterName = characterName, maxHealth = maxHealth, attack = attack, level = level };
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    public void Load()
    {
        if (!File.Exists(SavePath)) return;
        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        characterName = data.characterName;
        maxHealth = data.maxHealth;
        attack = data.attack;
        level = data.level;
    }

    public void DeleteSave() { if (File.Exists(SavePath)) File.Delete(SavePath); }
}
