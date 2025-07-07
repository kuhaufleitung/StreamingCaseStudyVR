using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class FieldManager : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField] private List<GameObject> gridCubes = new();
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;
    private readonly Color _defaultAltColor = Color.grey;

    [Header("Game Settings")]
    [SerializeField] private int pressesPerRound = 20;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationDuration = 2f;
    [SerializeField] private float minRotationAngle = 30f;
    [SerializeField] private float maxRotationAngle = 70f;

    [SerializeField] private GameObject hud;
    private AudioSource _audioSource;

    private DefaultInputXR _input;


    // Game state
    private GameObject _currentTarget;
    private int _correctSelections;
    private int _currentRound = 0;
    private int _pressesInRound = 0;
    private float _roundStartTime;
    private float _roundCompletionTime;
    private bool _isRoundActive = false;

    // Rotation state
    public Quaternion targetRotation { get; set; }
    public bool isRotating { get; set; }
    public float rotationTimer { get; set; }

    public event Action OnRoundStart;
    public event Action<float> OnRoundComplete;
    public event Action<int> OnPressUpdate;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        if (gridCubes.Count == 0)
        {
            Debug.LogError("No grid cubes assigned!");
            return;
        }
        _input = new DefaultInputXR();
        _input.Default.Enable();
        _input.Default.ResetView.started += ResetViewOnstarted;
    }

    public void StartNewRound()
    {
        if (_isRoundActive) return;
        _audioSource.PlayOneShot(CreateTone());
        _currentRound++;
        _pressesInRound = 0;
        _roundStartTime = Time.time;
        _isRoundActive = true;
        _correctSelections = 0;

        SelectRandomCube();
        SetNewRandomRotation();

        OnRoundStart?.Invoke();
    }

    private void CompleteRound()
    {
        _isRoundActive = false;
        _roundCompletionTime = Time.time - _roundStartTime;
        OnRoundComplete?.Invoke(_roundCompletionTime);
        Debug.Log($"Round completed in {_roundCompletionTime.ToString("F2")} seconds. " +
                 $"Accuracy: {(_correctSelections / (float)pressesPerRound) * 100f}%");
    }

    private void Update()
    {
        if (!_isRoundActive) return;

        if (isRotating)
        {
            RotateTiles();
        }
    }

    private void RotateTiles()
    {
        rotationTimer += Time.deltaTime;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        hud.transform.rotation = transform.rotation;

        if (rotationTimer >= rotationDuration ||
            Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            isRotating = false;
            transform.rotation = targetRotation;
            hud.transform.rotation = targetRotation;
        }
    }

    public void HandleCubeSelection(GameObject selectedCube)
    {
        if (!_isRoundActive || _currentTarget == null || isRotating) return;

        if (selectedCube != _currentTarget) return;

        _correctSelections++;
        _pressesInRound++;
        OnPressUpdate?.Invoke(_pressesInRound);

        if (_pressesInRound >= pressesPerRound)
        {
            CompleteRound();
            return;
        }

        StartRotationAndSelectNewCube();
    }

    private void StartRotationAndSelectNewCube()
    {
        SetNewRandomRotation();
        isRotating = true;
        rotationTimer = 0f;
        SelectRandomCube();
    }

    private void SetNewRandomRotation()
    {
        float randomAngle = UnityEngine.Random.Range(minRotationAngle, maxRotationAngle);
        if (UnityEngine.Random.value > 0.5f) randomAngle *= -1;
        targetRotation = transform.rotation * Quaternion.Euler(0, randomAngle, 0);
    }

    private void SelectRandomCube()
    {
        for (var i = 0; i < gridCubes.Count; i++)
        {
            gridCubes[i].GetComponent<Renderer>().material.color =
                i % 2 == 0 ? _defaultAltColor : defaultColor;
        }

        int randomIndex = UnityEngine.Random.Range(0, gridCubes.Count);
        _currentTarget = gridCubes[randomIndex];
        _currentTarget.GetComponent<Renderer>().material.color = targetColor;
    }

    public float GetCurrentRoundTime()
    {
        return _isRoundActive ? Time.time - _roundStartTime : 0f;
    }

    public int GetCurrentRound()
    {
        return _currentRound;
    }

    public bool IsRoundActive()
    {
        return _isRoundActive;
    }

    public int GetPressesPerRound()
    {
        return pressesPerRound;
    }
    void ResetViewOnstarted(InputAction.CallbackContext obj)
    {
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        foreach (var subsystem in subsystems)
        {
            subsystem?.TryRecenter();
        }
        XROrigin origin = FindObjectOfType<XROrigin>();
        origin?.MatchOriginUpCameraForward(Vector3.up, Vector3.forward);
    }

    AudioClip CreateTone(float frequency = 440f, float duration = 0.2f)
    {
        int samples = (int)(duration * AudioSettings.outputSampleRate);
        AudioClip clip = AudioClip.Create("Tone", samples, 1,
            AudioSettings.outputSampleRate, false);
        float[] data = new float[samples];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Mathf.Sin(2 * Mathf.PI * frequency * i /
                                AudioSettings.outputSampleRate);
        }
        clip.SetData(data, 0);
        return clip;
    }

}