using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookScript : MonoBehaviour
{
    private float rotX;
    private float rotY;

    public float sensitivity = 5f;

    public Camera mainCamera;

    private void Update() { 
        
        rotX += Input.GetAxis("Mouse X") * sensitivity;
        rotY += Input.GetAxis("Mouse Y") * sensitivity;

        rotY = Mathf.Clamp(rotY, -90, 90);

        mainCamera.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, rotX, 0f);
    }
}
