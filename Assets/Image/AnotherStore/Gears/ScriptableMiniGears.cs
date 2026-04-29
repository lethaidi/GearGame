using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "NewMiniGears", menuName = "Game/MiniGears")]
public class ScriptableMiniGears : ScriptableObject
{
    [Header("Trạng thái hiển thị")]
    public string locked = "Locked";
    public string unlocked = "Unlocked";
    public string upgrade = "Upgrade";

    [Header("Rank Sprites")]
    public Sprite rankA, rankB, rankS, gear, icon;

    [Header("Trạng thái lưu được")]
    public bool isUnlocked;
    public bool isUpgrade;
    public string savedText;
    public Sprite savedRank;
    public bool isTheLockDestroyed;
    public bool isLockGearDestroyed;
    public bool isLockIconDestroyed;
    public GameObject prefabGear;
    public int count, rank = 0;

    [System.Serializable]
    private class SaveData
    {
        public bool isUnlocked, isUpgrade;
        public string savedText;
        public int count, rank;
        public bool isTheLockDestroyed, isLockGearDestroyed, isLockIconDestroyed;
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, name + ".json");

    public void Save()
    {
        var data = new SaveData()
        {
            isUnlocked = isUnlocked,
            isUpgrade = isUpgrade,
            savedText = savedText,
            count = count,
            rank = rank,
            isTheLockDestroyed = isTheLockDestroyed,
            isLockGearDestroyed = isLockGearDestroyed,
            isLockIconDestroyed = isLockIconDestroyed
        };
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    public void Load()
    {
        if (!File.Exists(SavePath)) return;
        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        isUnlocked = data.isUnlocked;
        isUpgrade = data.isUpgrade;
        savedText = data.savedText;
        count = data.count;
        rank = data.rank;
        isTheLockDestroyed = data.isTheLockDestroyed;
        isLockGearDestroyed = data.isLockGearDestroyed;
        isLockIconDestroyed = data.isLockIconDestroyed;
    }

    public void DeleteSave() { if (File.Exists(SavePath)) File.Delete(SavePath); }
}
