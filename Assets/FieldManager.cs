using UnityEngine;
using System.Collections.Generic;

public class FieldManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> gridCubes = new List<GameObject>();
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;
    private readonly Color _defaultAltColor = Color.grey;

    private GameObject _currentTarget;
    private int _correctSelections;

    private void Start()
    {
        if (gridCubes.Count == 0)
        {
            Debug.LogError("No grid cubes assigned!");
            return;
        }
        SelectRandomCube();
    }

    // Call this method when a cube is pressed
    public void HandleCubeSelection(GameObject selectedCube)
    {
        if (_currentTarget == null) return;

        if (selectedCube != _currentTarget) return;
        
        _correctSelections++;
        Debug.Log($"Correct! Total correct: {_correctSelections}");
        SelectRandomCube();
    }

    private void SelectRandomCube()
    {
        // Reset all cubes to default color
        for (var i = 0; i < gridCubes.Count; i++)
        {
            if (i % 2 == 0)
            {
                gridCubes[i].GetComponent<Renderer>().material.color = _defaultAltColor;
            }
            else
            {
                gridCubes[i].GetComponent<Renderer>().material.color = defaultColor;
            }
        }

        // Choose a new random cube
        int randomIndex = Random.Range(0, gridCubes.Count);
        _currentTarget = gridCubes[randomIndex];
        _currentTarget.GetComponent<Renderer>().material.color = targetColor;
    }
}