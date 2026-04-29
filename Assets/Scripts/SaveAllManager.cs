using UnityEngine;
using System.Collections.Generic;

public class SaveAllManager : MonoBehaviour
{
    [Header("Skills")]
    public List<ScriptableSkills> skills;

    [Header("Mini Gears")]
    public List<ScriptableMiniGears> miniGears;

    [Header("Characters")]
    public List<CharacterStats> characters;

    [Header("Slot Gears")]
    public List<ScriptableSlotGear> slotGears;

    [Header("Banner Packs")]
    public List<BannerPack> bannerPacks;

    [Header("Tasks")]
    public List<TaskSO> tasks;

    private void Awake()
    {
        // Load tất cả khi vào game
        LoadAll();
    }
    private void OnEnable()
    {
        LoadAll(); // ScriptableObject tự load lại khi được Unity enable
    }
    private void OnApplicationQuit()
    {
        // Save tất cả khi thoát game
        SaveAll();
    }

    // 🔹 Lưu tất cả
    public void SaveAll()
    {
        foreach (var s in skills) s?.Save();
        foreach (var g in miniGears) g?.Save();
        foreach (var c in characters) c?.Save();
        foreach (var sg in slotGears) sg?.Save();
        foreach (var bp in bannerPacks) bp?.Save();
        foreach (var t in tasks) t?.Save();

        Debug.Log("[SaveAllManager] ✅ Saved all ScriptableObjects!");
    }

    // 🔹 Load tất cả
    public void LoadAll()
    {
        foreach (var s in skills) s?.Load();
        foreach (var g in miniGears) g?.Load();
        foreach (var c in characters) c?.Load();
        foreach (var sg in slotGears) sg?.Load();
        foreach (var bp in bannerPacks) bp?.Load();
        foreach (var t in tasks) t?.Load();

        Debug.Log("[SaveAllManager] ✅ Loaded all ScriptableObjects!");
    }
}
