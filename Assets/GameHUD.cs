using UnityEngine;
using TMPro;
using UnityEngine.XR;
using Unity.VisualScripting;

public class GameHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private FieldManager fieldManager;
    [SerializeField] private float hudDistance = 1.5f;
    [SerializeField] private float hudHeight = 0.3f;

    private void Awake()
    {
        if (fieldManager != null)
        {
            fieldManager.OnRoundStart += OnRoundStart;
            fieldManager.OnRoundComplete += OnRoundComplete;
            fieldManager.OnPressUpdate += UpdateProgress;
        }
    }

    private void Update()
    {
        if (fieldManager == null || !fieldManager.IsRoundActive()) return;

        timeText.text = $"Time: {fieldManager.GetCurrentRoundTime():F1}s";
    }
    private void OnRoundStart()
    {
        UpdateProgress(0);
    }

    private void OnRoundComplete(float time)
    {
        timeText.text = $"Completed: {time:F2}s";
    }

    private void UpdateProgress(int presses)
    {
        progressText.text = $"{presses}/{fieldManager.GetPressesPerRound()}";
    }

    private void OnDestroy()
    {
        if (fieldManager != null)
        {
            fieldManager.OnRoundStart -= OnRoundStart;
            fieldManager.OnRoundComplete -= OnRoundComplete;
            fieldManager.OnPressUpdate -= UpdateProgress;
        }
    }
}