//  PlayerMovement.cs - Script for enabling Player to move, as well as performing gravity checks.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class PlayerMovement : MonoBehaviour
{
    //  VARIABLES:
    public CharacterController  Controller;
    public CameraMovement              CameraController;
    public World                CurrentWorld;

    public Vector3  Velocity;
    public float    MoveSpeed   = 5.0f;
    public float    JumpPower   = 2.0f;

    //  FUNCTIONS:
    //  Function to update the script during runtime.
    void Update()
    {
        // Set velocity to negative so Player stays touching the ground.
        if (Controller.isGrounded && Velocity.y < 0.0f)
        {
            Velocity.y = -2.0f;
        }

        if (!CameraController.IsCursorHidden)
        {
            //  Add jump velocity to enable jumping if grounded.
            if (Input.GetButtonDown("Jump") && Controller.isGrounded)
            {
                Velocity.y = Mathf.Sqrt(JumpPower * -2.0f * CurrentWorld.Gravity);
            }

            //  Get Player input from axis.
            float xDirection = Input.GetAxis("Horizontal");
            float zDirection = Input.GetAxis("Vertical");

            //  Move Player by movement input.
            Vector3 moveDirection = transform.right * xDirection + transform.forward * zDirection;
            Controller.Move(moveDirection * MoveSpeed * Time.deltaTime);
        }

        //  Move Player by current gravity velocity.
        Velocity.y += CurrentWorld.Gravity * Time.deltaTime;
        Controller.Move(Velocity * Time.deltaTime);
    }
}