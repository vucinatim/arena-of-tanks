using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementData
{
    public float maxSpeed;
    public float acceleration;
    public float rotateSpeed;
}

public class MovementSystem : MonoBehaviour
{
    private Player owner;

    [SerializeField] private MovementData data;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private LayerMask groundMask;

    private float maxSpeed;
    private float acceleration;
    private float rotateSpeed;
    private bool isGrounded = true;
    private float speedModifier = 1f;
    private Rigidbody rb;

    private TankAnimationSystem tankAnimationSystem;

    private RigidbodyConstraints defaultRigidbodyConstraints;

    public void Initialize(Player owner, MovementData data)
    {
        this.owner = owner;
        this.data = data;

        maxSpeed = data.maxSpeed;
        acceleration = data.acceleration;
        rotateSpeed = data.rotateSpeed;

        tankAnimationSystem = GetComponent<TankAnimationSystem>();

        // Rigidbody setup
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1f, 0);
        defaultRigidbodyConstraints = rb.constraints;
    }

    public void ResetForRespawn()
    {
        maxSpeed = data.maxSpeed;
        acceleration = data.acceleration;
        rotateSpeed = data.rotateSpeed;
        speedModifier = 1f;
    }

    public void SetMaxSpeed()
    {
        maxSpeed = data.maxSpeed * speedModifier;
        EventManager.TriggerSpeedChanged(owner, maxSpeed, data.maxSpeed);
    }

    public void SetRotateSpeed(float newRotateSpeed)
    {
        rotateSpeed = newRotateSpeed;
        EventManager.TriggerRotateSpeedChanged(owner, rotateSpeed, data.rotateSpeed);
    }

    public void ChangeSpeedModifier(float modifier)
    {
        Debug.Log("Adding speed modifier: " + modifier);
        speedModifier += modifier;
        SetMaxSpeed();
    }

    public void Move(Transform transform, float verticalInput, float horizontalInput)
    {
        CheckIfGrounded();

        if (isGrounded)
        {
            // Rotate the tank based on horizontal input
            float rotationAngle = horizontalInput * rotateSpeed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.Euler(0f, rotationAngle, 0f);
            transform.rotation *= targetRotation;

            // Calculate the movement direction based on the current rotation of the tank
            Vector3 movementDirection = transform.forward * verticalInput;

            // Normalize the movement direction
            movementDirection = movementDirection.normalized;

            // Calculate the desired movement position
            Vector3 desiredMovement = transform.position + movementDirection * maxSpeed * Time.deltaTime;

            // Check if the movement would cause a collision
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, movementDirection, out hit, maxSpeed * Time.deltaTime))
            {
                // If there's no collision, apply the velocity to the rigidbody
                Vector3 velocity = movementDirection * maxSpeed;
                rb.velocity = velocity;

                // Animate the tank
                tankAnimationSystem.AnimateMovement(verticalInput, horizontalInput, velocity);
            }
            else
            {
                // If there's a collision, apply a small force to push the tank away from the wall
                Vector3 pushAwayForce = hit.normal * maxSpeed * Time.deltaTime;
                rb.AddForce(pushAwayForce, ForceMode.Impulse);

                // Stop the forward movement
                Vector3 velocity = new Vector3(0, rb.velocity.y, 0);
                rb.velocity = velocity;
            }
        }
        else
        {
            Debug.Log("Not grounded");
        }
    }


    private void CheckIfGrounded()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundMask);
    }

    public void Root()
    {
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Unroot()
    {
        transform.GetComponent<Rigidbody>().constraints = defaultRigidbodyConstraints;
    }
}
