using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class InputLogic : MonoBehaviour
{
    private Defaultlnput _input;
    // Start is called before the first frame update
    void Start()
    {
        _input = new Defaultlnput();
        _input.Default.Enable();
        _input.Default.ResetView.started += ResetViewOnstarted;
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
}
