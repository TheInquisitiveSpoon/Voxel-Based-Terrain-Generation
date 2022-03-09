using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public GameObject groundScanner;
    public World currentWorld;
    public LayerMask groundLayer;

    public float footRadius = 0.4f;
    public float moveSpeed = 5.0f;
    public float JumpPower = 2.0f;
    public Vector3 Velocity;
    public bool isGrounded = false;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundScanner.transform.position, footRadius, groundLayer);

        if (isGrounded && Velocity.y < 0.0f)
        {
            Velocity.y = -2.0f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Velocity.y = Mathf.Sqrt(JumpPower * -2.0f * currentWorld.gravity);
        }

        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * xDirection + transform.forward * zDirection;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        Velocity.y += currentWorld.gravity * Time.deltaTime;
        controller.Move(Velocity * Time.deltaTime);
    }
}