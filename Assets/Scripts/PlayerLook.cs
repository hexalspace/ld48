using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// playerBody is the container for all the player related objects
// playerCamera is the camera that should be a child of playerBody so the rotation of body affects camera
public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivityLeftRight;
    public float mouseSensitivityUpDown;
    public Transform playerBody;
    public Transform playerCamera;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        var leftRight = Input.GetAxis("LookLeftRight") * mouseSensitivityLeftRight * Time.deltaTime;
        var upDown = Input.GetAxis("LookUpDown") * mouseSensitivityUpDown * Time.deltaTime;

        xRotation -= upDown;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.transform.Rotate(Vector3.up * leftRight);
    }
}
