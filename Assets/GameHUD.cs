using UnityEngine;
using TMPro;
using UnityEngine.XR;
using Unity.VisualScripting;

public class GameHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private FieldManager fieldManager;
    [SerializeField] private Canvas hudPanel;
    [SerializeField] private float hudDistance = 1.5f;
    [SerializeField] private float hudHeight = 0.3f;

    private Camera _mainCamera;
    private bool _isXR;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _isXR = XRSettings.enabled;

        if (fieldManager != null)
        {
            fieldManager.OnRoundStart += OnRoundStart;
            fieldManager.OnRoundComplete += OnRoundComplete;
            fieldManager.OnPressUpdate += UpdateProgress;
        }
    }

    private void Update()
    {
        if (!_isXR || fieldManager == null || !fieldManager.IsRoundActive()) return;

        timeText.text = $"Time: {fieldManager.GetCurrentRoundTime():F1}s";

        PositionHUD();
    }

    private void PositionHUD()
    {
        if (_mainCamera == null) return;

        Vector3 hudPosition = _mainCamera.transform.position +
                              _mainCamera.transform.forward * hudDistance;

        hudPosition += _mainCamera.transform.up * hudHeight;

        hudPanel.transform.position = Vector3.Lerp(
            hudPanel.transform.position,
            hudPosition,
            Time.deltaTime * 10f // Adjust smoothness
        );

        Quaternion targetRotation = Quaternion.LookRotation(
            _mainCamera.transform.position - hudPanel.transform.position,
            Vector3.up
        );

        hudPanel.transform.rotation = targetRotation * Quaternion.Euler(0, 180, 0);
    }

    private void OnRoundStart()
    {
        roundText.text = $"Round: {fieldManager.GetCurrentRound()}";
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