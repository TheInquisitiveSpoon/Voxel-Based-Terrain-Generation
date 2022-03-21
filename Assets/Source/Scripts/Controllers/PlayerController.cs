//  PlayerMovement.cs - Script for enabling Player to move, as well as performing gravity checks.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class PlayerController : MonoBehaviour
{
    //  VARIABLES:
    public CharacterController  Controller;
    public CameraController       CameraController;
    public World                CurrentWorld;

    public Vector3  Velocity;
    public float    MoveSpeed   = 5.0f;
    public float    JumpPower   = 2.0f;
    public bool     IsFlyEnabled = false;

    //  FUNCTIONS:
    //  Function to update the script during runtime.
    void Update()
    {
        if (!IsFlyEnabled)
        {
            // Set velocity to negative so Player stays touching the ground.
            if (Controller.isGrounded && Velocity.y < 0.0f)
            {
                Velocity.y = -2.0f;
            }
        }

        if (!CameraController.IsCursorHidden)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                SwapFlyEnabled();
            }

            //  Get Player input from axis.
            float xDirection = Input.GetAxis("Horizontal");
            float zDirection = Input.GetAxis("Vertical");

            if (!IsFlyEnabled)
            {
                //  Add jump velocity to enable jumping if grounded.
                if (Input.GetButtonDown("Jump") && Controller.isGrounded)
                {
                    Velocity.y = Mathf.Sqrt(JumpPower * -2.0f * CurrentWorld.Gravity);
                }

                //  Move Player by movement input.
                Vector3 moveDirection = transform.right * xDirection + transform.forward * zDirection;
                if (Input.GetKey(KeyCode.LeftShift)) { Controller.Move(moveDirection * (2 * MoveSpeed) * Time.deltaTime); }
                else { Controller.Move(moveDirection * MoveSpeed * Time.deltaTime); }
            }
            else
            {
                float yDirection = 0.0f;
                if (Input.GetButton("Jump")) { yDirection = 1.0f; }
                else if (Input.GetKey(KeyCode.LeftAlt)) { yDirection = -1.0f; }

                Vector3 moveDirection = transform.right * xDirection + transform.up * yDirection + transform.forward * zDirection;
                if (Input.GetKey(KeyCode.LeftShift)) { Controller.Move(moveDirection * (4 * MoveSpeed) * Time.deltaTime); }
                else { Controller.Move(moveDirection * (2 * MoveSpeed) * Time.deltaTime); }
            }
        }

        if (!IsFlyEnabled)
        {
            //  Move Player by current gravity velocity.
            Velocity.y += CurrentWorld.Gravity * Time.deltaTime;
            Controller.Move(Velocity * Time.deltaTime);
        }
    }

    public void SwapFlyEnabled()
    {
        if (IsFlyEnabled)   { IsFlyEnabled = false; Velocity.y = 0.0f; }
        else                { IsFlyEnabled = true; Velocity.y = 0.0f; }
    }
}