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

        // Subscribe to events
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

        // Update time display continuously
        timeText.text = $"Time: {fieldManager.GetCurrentRoundTime():F1}s";

        // Position HUD in VR/AR space
        PositionHUD();
    }

    private void PositionHUD()
    {
        if (_mainCamera == null) return;

        // Calculate position with vertical offset
        Vector3 hudPosition = _mainCamera.transform.position +
                              _mainCamera.transform.forward * hudDistance;

        // Apply vertical offset relative to camera's up direction
        hudPosition += _mainCamera.transform.up * hudHeight;

        // Smooth movement (optional)
        hudPanel.transform.position = Vector3.Lerp(
            hudPanel.transform.position,
            hudPosition,
            Time.deltaTime * 10f // Adjust smoothness
        );

        // Match camera's pitch and yaw, but ignore roll
        Quaternion targetRotation = Quaternion.LookRotation(
            _mainCamera.transform.position - hudPanel.transform.position,
            Vector3.up
        );

        // Flip 180 degrees to face player
        hudPanel.transform.rotation = targetRotation * Quaternion.Euler(0, 180, 0);
    }

    private void OnRoundStart()
    {
        roundText.text = $"Round: {fieldManager.GetCurrentRound()}";
        UpdateProgress(0);
    }

    private void OnRoundComplete(float time)
    {
        // Optional: Show final time differently
        timeText.text = $"Completed: {time:F2}s";
    }

    private void UpdateProgress(int presses)
    {
        progressText.text = $"{presses}/{fieldManager.GetPressesPerRound()}";
    }

    private void OnDestroy()
    {
        // Clean up event subscriptions
        if (fieldManager != null)
        {
            fieldManager.OnRoundStart -= OnRoundStart;
            fieldManager.OnRoundComplete -= OnRoundComplete;
            fieldManager.OnPressUpdate -= UpdateProgress;
        }
    }
}