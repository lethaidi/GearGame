using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "NewSlotGear", menuName = "Game/SlotGear")]
public class ScriptableSlotGear : ScriptableObject
{
    public GameObject Slot;       // Prefab reference
    public bool isEquipped = false;

    [System.Serializable]
    private class SaveData
    {
        public bool isEquipped;
        public string slotName;   // Lưu tên prefab
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, name + ".json");

    public void Save()
    {
        var data = new SaveData()
        {
            isEquipped = isEquipped,
            slotName = Slot != null ? Slot.name : ""
        };

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    public void Load()
    {
        if (!File.Exists(SavePath)) return;

        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        isEquipped = data.isEquipped;

        if (!string.IsNullOrEmpty(data.slotName))
        {
            // Load lại từ Resources và gán vào Slot
            Slot = Resources.Load<GameObject>("GearPrefab/Respawned/" + data.slotName);

            if (Slot == null)
            {
                Debug.LogError("Không tìm thấy prefab: " + data.slotName);
            }
        }
    }


    public void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }
}
