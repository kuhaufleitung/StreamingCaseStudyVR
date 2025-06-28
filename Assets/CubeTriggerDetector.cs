using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CubeTriggerDetector : MonoBehaviour
{
    private FieldManager _fieldManager;

    private void Start()
    {
        _fieldManager = FindObjectOfType<FieldManager>();
        if (_fieldManager == null)
        {
            Debug.LogError("FieldManager not found in scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsXRController(other.gameObject))
        {
            _fieldManager?.HandleCubeSelection(gameObject);
        }
    }

    private bool IsXRController(GameObject obj)
    {
        return obj.CompareTag("XRController") ||
               obj.CompareTag("Hand") ||
               obj.GetComponent<XRController>() != null;
    }
}