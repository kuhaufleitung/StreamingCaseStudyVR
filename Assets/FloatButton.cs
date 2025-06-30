using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatButton : MonoBehaviour
{
    [Header("Positioning")]
    [SerializeField] private float verticalOffset = -0.3f; // How far below the camera
    [SerializeField] private float distanceFromCamera = 0.5f; // How far in front

    private Transform _cameraTransform;
    private Vector3 _initialWorldHeight;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _initialWorldHeight = new Vector3(0, transform.position.y, 0); // Lock initial height
    }

    private void LateUpdate()
    {
        // Match camera's X/Z position but maintain world Y position
        Vector3 targetPosition = new Vector3(
            _cameraTransform.position.x,
            _initialWorldHeight.y + verticalOffset,
            _cameraTransform.position.z
        ) + _cameraTransform.forward * distanceFromCamera;

        transform.position = targetPosition;

        // Optional: Make button face camera while staying upright
        transform.rotation = Quaternion.Euler(
            0,
            _cameraTransform.eulerAngles.y,
            0
        );
    }
}
