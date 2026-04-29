using UnityEngine;
using TMPro;

public class TaskUI : MonoBehaviour
{
    public TaskSO taskData;

    [Header("UI Elements")]
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;
    public GameObject buttonGreen;
    public GameObject buttonGrey;
    public GameObject buttonIsReceive;

    public void InitUI()
    {
        descriptionText.text = taskData.description;
        progressText.text = taskData.currentValue + "/" + taskData.targetValue;

        buttonGreen.SetActive(false);
        buttonGrey.SetActive(true);
        buttonIsReceive.SetActive(false);
    }

    public void ResetUI()
    {
        taskData.ResetProgress();
        InitUI();
    }
}
