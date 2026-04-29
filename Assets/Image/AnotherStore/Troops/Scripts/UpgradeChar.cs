using UnityEngine;
using UnityEngine.UI;

public class UpgradeChar : MonoBehaviour
{
    public CharacterStats characterStats;
    public Sprite imgLv1, imgLv2, imgLv3;
    public Image ava;

    void Update()
    {
        Upgrade();
    }

    void Upgrade()
    {
        if (characterStats == null || ava == null)
        {
            Debug.LogError("Chưa gán characterStats hoặc ava trong Inspector!");
            return;
        }

        if (characterStats.level >= 1 && characterStats.level <= 9)
        {
            ava.sprite = imgLv1;
        }
        else if (characterStats.level >= 10 && characterStats.level <= 19)
        {
            ava.sprite = imgLv2;
        }
        else if (characterStats.level >= 20)
        {
            ava.sprite = imgLv3;
        }
    }
}
