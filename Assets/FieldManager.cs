using UnityEngine;
using System.Collections.Generic;

public class FieldManager : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField] private List<GameObject> gridCubes = new List<GameObject>();
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;
    private readonly Color _defaultAltColor = Color.grey;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationDuration = 2f; 
    [SerializeField] private float minRotationAngle = 45f;
    [SerializeField] private float maxRotationAngle = 90f;

    private GameObject _currentTarget;
    private int _correctSelections;
    private Quaternion _targetRotation;
    private bool _isRotating;
    private float _rotationTimer;

    private void Start()
    {
        if (gridCubes.Count == 0)
        {
            Debug.LogError("No grid cubes assigned!");
            return;
        }
        SelectRandomCube();
        SetNewRandomRotation();
    }

    private void Update()
    {
        if (_isRotating)
        {
            RotateTiles();
        }
    }

    private void RotateTiles()
    {
        _rotationTimer += Time.deltaTime;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            _targetRotation,
            rotationSpeed * Time.deltaTime
        );

        if (_rotationTimer >= rotationDuration ||
            Quaternion.Angle(transform.rotation, _targetRotation) < 0.1f)
        {
            _isRotating = false;
            transform.rotation = _targetRotation;
        }
    }

    public void HandleCubeSelection(GameObject selectedCube)
    {
        if (_currentTarget == null || _isRotating) return;

        if (selectedCube != _currentTarget) return;

        _correctSelections++;
        Debug.Log($"Correct! Total correct: {_correctSelections}");
        StartRotationAndSelectNewCube();
    }

    private void StartRotationAndSelectNewCube()
    {
        SetNewRandomRotation();
        _isRotating = true;
        _rotationTimer = 0f;
        // Delay selection until rot is done
        Invoke(nameof(SelectRandomCube), rotationDuration * 0.8f);
    }

    private void SetNewRandomRotation()
    {
        float randomAngle = Random.Range(minRotationAngle, maxRotationAngle);
        if (Random.value > 0.5f) randomAngle *= -1; // 50% chance to rotate opposite direction

        _targetRotation = transform.rotation * Quaternion.Euler(0, randomAngle, 0);
    }

    private void SelectRandomCube()
    {
        // Reset all cubes to default color
        for (var i = 0; i < gridCubes.Count; i++)
        {
            gridCubes[i].GetComponent<Renderer>().material.color =
                i % 2 == 0 ? _defaultAltColor : defaultColor;
        }

        int randomIndex = Random.Range(0, gridCubes.Count);
        _currentTarget = gridCubes[randomIndex];
        _currentTarget.GetComponent<Renderer>().material.color = targetColor;
    }
}