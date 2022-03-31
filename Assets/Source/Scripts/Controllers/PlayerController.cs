//  PlayerMovement.cs - Script for enabling Player to move, as well as performing gravity checks.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class PlayerController : MonoBehaviour
{
    //  REFERENCES:
    public CharacterController  Controller;
    public CameraController     CameraController;
    public World                CurrentWorld;

    //  VARIABLES:
    private Vector3     Velocity;
    public float        MoveSpeed       = 5.0f;
    public float        JumpPower       = 2.0f;
    private bool        IsFlyEnabled    = true;

    //  FUNCTIONS:
    //  Function to update the script during runtime.
    void Update()
    {
        //  Terminates program.
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Application.Quit();
        }

        //  Enables gravity if flying is disabled.
        if (!IsFlyEnabled)
        {
            // Set velocity to negative so Player stays touching the ground.
            if (Controller.isGrounded && Velocity.y < 0.0f) { Velocity.y = -2.0f; }
        }

        //  Enables player movement when the cursor is not locked.
        if (!CameraController.IsCursorHidden)
        {
            //  Toggles flying.
            if (Input.GetKeyDown(KeyCode.F1)) { SwapFlyEnabled(); }

            //  Get Player input from axis.
            float xDirection = Input.GetAxis("Horizontal");
            float zDirection = Input.GetAxis("Vertical");

            //  Handles player movement with and without flying.
            if (!IsFlyEnabled)
            {
                //  Add jump velocity to enable jumping if grounded.
                if (Input.GetButtonDown("Jump") && Controller.isGrounded)
                {
                    Velocity.y = Mathf.Sqrt(JumpPower * -2.0f * CurrentWorld.Gravity);
                }

                //  Move Player by movement input.
                Vector3 moveDirection = transform.right * xDirection + transform.forward * zDirection;
                
                //  Adds increased movement speed if player is holding Left Shift.
                if (Input.GetKey(KeyCode.LeftShift))    { Controller.Move(moveDirection * (2 * MoveSpeed) * Time.deltaTime); }
                else                                    { Controller.Move(moveDirection * MoveSpeed * Time.deltaTime); }
            }
            else
            {
                //  Player Y movement variable.
                float yDirection = 0.0f;

                //  Changes player altitude based on input.
                if (Input.GetButton("Jump"))                { yDirection = 1.0f; }
                else if (Input.GetKey(KeyCode.LeftAlt))     { yDirection = -1.0f; }

                //  Vector to store new position of player.
                Vector3 moveDirection = transform.right * xDirection + transform.up * yDirection + transform.forward * zDirection;

                //  Adds increased movement speed if the player is holding Left Shift.
                if (Input.GetKey(KeyCode.LeftShift))    { Controller.Move(moveDirection * (4 * MoveSpeed) * Time.deltaTime); }
                else                                    { Controller.Move(moveDirection * (2 * MoveSpeed) * Time.deltaTime); }
            }
        }

        //  Adds gravity to player if not flying.
        if (!IsFlyEnabled)
        {
            //  Move Player by current gravity velocity.
            Velocity.y += CurrentWorld.Gravity * Time.deltaTime;
            Controller.Move(Velocity * Time.deltaTime);
        }
    }

    //  Toggles flying.
    private void SwapFlyEnabled()
    {
        if (IsFlyEnabled)   { IsFlyEnabled = false; Velocity.y = 0.0f; }
        else                { IsFlyEnabled = true;  Velocity.y = 0.0f; }
    }
}