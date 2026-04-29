using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "NewBannerPack", menuName = "Game/BannerPack")]
public class BannerPack : ScriptableObject
{
    public string packName;
    public int packRank;
    public int currentExp;
    public int expToNextLevel;

    [System.Serializable]
    private class SaveData
    {
        public string packName;
        public int packRank;
        public int currentExp;
        public int expToNextLevel;
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, name + ".json");

    public void Save()
    {
        var data = new SaveData()
        {
            packName = packName,
            packRank = packRank,
            currentExp = currentExp,
            expToNextLevel = expToNextLevel
        };
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    public void Load()
    {
        if (!File.Exists(SavePath)) return;

        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        packName = data.packName;
        packRank = data.packRank;
        currentExp = data.currentExp;
        expToNextLevel = data.expToNextLevel;
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}
