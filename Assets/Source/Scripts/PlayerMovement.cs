//  PlayerMovement.cs - Script for enabling Player to move, as well as performing gravity checks.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class PlayerMovement : MonoBehaviour
{
    //  VARIABLES:
    public CharacterController  Controller;
    public GameObject           GroundScanner;
    public World                CurrentWorld;
    public LayerMask            GroundLayer;

    public Vector3  Velocity;
    public float    FootRadius  = 0.4f;
    public float    MoveSpeed   = 5.0f;
    public float    JumpPower   = 2.0f;
    public bool     IsGrounded  = false;

    //  FUNCTIONS:
    //  Function to update the script during runtime.
    void Update()
    {
        //  Check to see if Player is touching the ground.
        IsGrounded = Physics.CheckSphere(GroundScanner.transform.position, FootRadius, GroundLayer);

        // Set velocity to negative so Player stays touching the ground.
        if (IsGrounded && Velocity.y < 0.0f)
        {
            Velocity.y = -2.0f;
        }

        //  Add jump velocity to enable jumping if grounded.
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            Velocity.y = Mathf.Sqrt(JumpPower * -2.0f * CurrentWorld.gravity);
        }

        //  Get Player input from axis.
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        //  Move Player by movement input.
        Vector3 moveDirection = transform.right * xDirection + transform.forward * zDirection;
        Controller.Move(moveDirection * MoveSpeed * Time.deltaTime);

        //  Move Player by current gravity velocity.
        Velocity.y += CurrentWorld.gravity * Time.deltaTime;
        Controller.Move(Velocity * Time.deltaTime);
    }
}