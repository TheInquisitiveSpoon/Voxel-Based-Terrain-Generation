//  CameraMovement.cs - Handles movement and other functions of the camera during gameplay.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class CameraController : MonoBehaviour
{
    //  REFERENCES:
    public GameObject   Player;

    //  VARIABLES:
    private float       CurrentXRotation    = 0.0f;
    private float       CurrentYRotation    = 0.0f;
    private float       MinRotationClamp    = -80.0f;
    private float       MaxRotationClamp    = 80.0f;
    public float        SensitivityX = 200.0f;
    public float        SensitivityY = 200.0f;

    private bool        IsFirstPerson       = true;
    private bool        IsInvertedY         = false;
    public bool         IsCursorHidden      = false;

    //  FUNCTIONS:
    // Function to update the script during runtime.
    void Update()
    {
        //  Swaps camera mode when F5 pressed.
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SwapCameraMode();
        }

        //  Changes cursor visibility when Escape pressed.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowHideCursor();
        }

        //  Enables camera rotation when the cursor is not locked.
        if (!IsCursorHidden)
        {
            //  Gets Movement input for X axis.
            float mouseX = Input.GetAxis("Mouse X") * SensitivityX * Time.deltaTime;

            //  Handles camera Y movement in first person, using clamping to keep in range of view.
            if (IsFirstPerson)
            {
                //  Gets Rotation input from mouse Y;
                float mouseY = Input.GetAxis("Mouse Y") * SensitivityY * Time.deltaTime;

                //  Moves camera for inverted and non-inverted camera.
                if (!IsInvertedY)   { CurrentYRotation -= mouseY; }
                else                { CurrentYRotation += mouseY; }

                //  Clamps camera rotation within acceptable range.
                CurrentYRotation = Mathf.Clamp(CurrentYRotation, MinRotationClamp, MaxRotationClamp);

                //  Rotates camera by new rotation.
                transform.localRotation = Quaternion.Euler(CurrentYRotation, 0.0f, 0.0f);
            }

            //  Rotates using mouse X Input.
            CurrentXRotation += mouseX;
            Player.transform.Rotate(Vector3.up * mouseX);
        }
    }

    //  Function to enable swapping between first and third person.
    internal void SwapCameraMode()
    {
        if (IsFirstPerson)
        {
            //  Places camera in position for first person.
            transform.localPosition = new Vector3(0.0f, 1.0f, -2.5f);
            transform.localRotation = Quaternion.AngleAxis(25.0f, Vector3.right);
            IsFirstPerson           = false;
        }
        else
        {
            //  Places camera in position for third person.
            transform.localPosition = new Vector3(0.0f, 0.4f, 0.1f);
            transform.rotation      = Player.transform.rotation;
            IsFirstPerson           = true;
        }
    }

    //  Function to hide or show mouse cursor.
    internal void ShowHideCursor()
    {
        if (IsCursorHidden)
        {
            Cursor.lockState = CursorLockMode.Locked;
            IsCursorHidden = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            IsCursorHidden = true;
        }
    }
}
