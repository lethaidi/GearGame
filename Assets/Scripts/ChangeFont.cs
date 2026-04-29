#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class ChangeAllTMPFont : EditorWindow
{
    private TMP_FontAsset newFont;

    [MenuItem("Tools/Change All TMP Fonts")]
    public static void ShowWindow()
    {
        GetWindow<ChangeAllTMPFont>("Change All TMP Fonts");
    }

    private void OnGUI()
    {
        GUILayout.Label("Đổi toàn bộ TextMeshPro Font trong Scene", EditorStyles.boldLabel);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Font mới", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Thay đổi tất cả"))
        {
            if (newFont == null)
            {
                EditorUtility.DisplayDialog("Lỗi", "Bạn cần chọn TMP_FontAsset trước!", "OK");
                return;
            }

            ChangeFontsInScene();
        }
    }

    private void ChangeFontsInScene()
    {
        int count = 0;
        TMP_Text[] allTexts = FindObjectsOfType<TMP_Text>(true); // true: bao gồm cả các object bị disable

        foreach (var tmp in allTexts)
        {
            Undo.RecordObject(tmp, "Change TMP Font");
            tmp.font = newFont;
            EditorUtility.SetDirty(tmp);
            count++;
        }

        EditorUtility.DisplayDialog("Hoàn tất", $"Đã đổi font cho {count} TextMeshPro object.", "OK");
    }
}
#endif
