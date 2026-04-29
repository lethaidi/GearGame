using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "NewTask", menuName = "Game/Task")]
public class TaskSO : ScriptableObject
{
    [Header("Thông tin nhiệm vụ")]
    public string description;

    [Header("Cài đặt Task")]
    public int targetValue;
    public int currentValue = 0;

    [Header("Phần thưởng")]
    public int rewardCoins;

    [HideInInspector] public bool hasClaimedReward = false;

    public bool isCompleted => currentValue >= targetValue;

    public void ResetProgress()
    {
        currentValue = 0;
        hasClaimedReward = false;
    }

    [System.Serializable]
    private class SaveData
    {
        public string description;
        public int targetValue, currentValue, rewardCoins;
        public bool hasClaimedReward;
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, name + ".json");

    public void Save()
    {
        var data = new SaveData()
        {
            description = description,
            targetValue = targetValue,
            currentValue = currentValue,
            rewardCoins = rewardCoins,
            hasClaimedReward = hasClaimedReward
        };
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    public void Load()
    {
        if (!File.Exists(SavePath)) return;
        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        description = data.description;
        targetValue = data.targetValue;
        currentValue = data.currentValue;
        rewardCoins = data.rewardCoins;
        hasClaimedReward = data.hasClaimedReward;
    }

    public void DeleteSave() { if (File.Exists(SavePath)) File.Delete(SavePath); }
}
