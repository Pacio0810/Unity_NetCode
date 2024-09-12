using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Camera Settings")]
    [SerializeField] private float MouseSensitivity = 100.0f;
    [SerializeField] private Transform PlayerBody;

    private float Xrotation = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float MouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        Xrotation -= MouseY;
        Xrotation = Mathf.Clamp(Xrotation, -90.0f, 90.0f);

        transform.localRotation = Quaternion.Euler(Xrotation, 0.0f, 0.0f);
        PlayerBody.Rotate(Vector3.up * MouseX);
    }
}
