using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 20f;

    private Vector2 movementVector;

    private PlayerInput input;
    private CharacterController controller;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();

        //if (Input.GetKey(KeyCode.Mouse1))
        //{
        //    Aim();
        //}
        //else
        //{
        //    Rotate();
        //}
    }

    private void Move()
    {
        // Get the movement vector from input
        movementVector = input.GetMovementVector();

        // Move the player using the CharacterController
        controller.Move(movementVector * moveSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        // Rotate the player to face the movement direction smoothly
        if (movementVector != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movementVector.y, movementVector.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, targetAngle));

            // Smoothly interpolate the rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void Aim()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(input.GetMousePosition());
        mousePosition.z = transform.position.z; // Ensure z-coordinate is the same as the player's position

        // Calculate the direction to the mouse position from the player's position
        Vector2 lookDirection = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        // Calculate the angle and set the rotation to face the mouse
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
