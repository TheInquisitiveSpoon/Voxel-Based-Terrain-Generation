using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;

    public float currentXRotation = 0.0f;
    public float currentYRotation = 0.0f;

    public float sensitivityX = 200.0f;
    public float sensitivityY = 200.0f;
    bool isFirstPerson = true;
    bool isInvertedY = false;
    bool isCursorHidden = false;

    public void SwapCameraMode()
    {
        if (isFirstPerson)
        {
            transform.localPosition = new Vector3(0.0f, 1.0f, -2.5f);
            transform.localRotation = Quaternion.AngleAxis(25.0f, Vector3.right);
            isFirstPerson = false;
        }
        else
        {
            transform.localPosition = new Vector3(0.0f, 0.4f, 0.1f);
            transform.rotation = player.transform.rotation;
            isFirstPerson = true;
        }
    }

    public void ShowHideCursor()
    {
        if (isCursorHidden)
        {
            Cursor.lockState = CursorLockMode.Locked;
            isCursorHidden = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            isCursorHidden = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SwapCameraMode();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowHideCursor();
        }

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;

        if (isFirstPerson)
        {
            float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;
            if (!isInvertedY) { currentYRotation -= mouseY; }
            else { currentYRotation += mouseY; }
            currentYRotation = Mathf.Clamp(currentYRotation, -80.0f, 80.0f);
            transform.localRotation = Quaternion.Euler(currentYRotation, 0.0f, 0.0f);
        }

        currentXRotation += mouseX;
        player.transform.Rotate(Vector3.up * mouseX);
    }
}
