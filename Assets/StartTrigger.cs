using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class StartTrigger : MonoBehaviour
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
            _fieldManager?.StartNewRound();
        }
    }

    private bool IsXRController(GameObject obj)
    {
        return obj.CompareTag("XRController") ||
               obj.CompareTag("Hand") ||
               obj.GetComponent<XRController>() != null;
    }
}
