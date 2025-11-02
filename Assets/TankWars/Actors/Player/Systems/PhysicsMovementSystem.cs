using UnityEngine;

[System.Serializable]
public class PhysicsMovementData
{
    public float maxSpeed;
    public float movementAcceleration;
    public float rotationSpeed;
    public float suspensionForce;
    public float suspensionDamp;
    public float restDistance;
    public float weightTransferSpeed;
    public float weightTransferAmount;
    public float weightTransferResetTime;
    public float wheelRotationSpeed;
}

public class TankWheels
{
    public Transform leftFront;
    public Transform rightFront;
    public Transform leftBack;
    public Transform rightBack;
    public Transform leftFrontVisual;
    public Transform rightFrontVisual;
    public Transform leftBackVisual;
    public Transform rightBackVisual;
}

public class PhysicsMovementSystem : MonoBehaviour
{
    private Player owner;
    private Rigidbody rb;

    [SerializeField] private PhysicsMovementData data;

    private float maxSpeed;

    private TankWheels wheels;
    private Transform[] raycastOrigins;

    private float frontSuspensionForce;
    private float backSuspensionForce;
    private float leftSuspensionForce;
    private float rightSuspensionForce;

    private float verticalWeightTransferResetTimer = 0f;
    private float horizontalWeightTransferResetTimer = 0f;
    private float lastVerticalInput = 0f;
    private float lastHorizontalInput = 0f;

    public void Initialize(Player owner, PhysicsMovementData data)
    {
        this.owner = owner;
        this.data = data;
        maxSpeed = data.maxSpeed;

        rb = owner.GetComponent<Rigidbody>();

        var playerTank = transform.Find("PlayerTank").gameObject;
        wheels = new TankWheels
        {
            leftBack = playerTank.transform.Find("Wheel_b_left"),
            leftFront = playerTank.transform.Find("Wheel_f_left"),
            rightBack = playerTank.transform.Find("Wheel_b_right"),
            rightFront = playerTank.transform.Find("Wheel_f_right")
        };
        wheels.leftBackVisual = wheels.leftBack.transform.Find("TankFree_Wheel_b_left");
        wheels.leftFrontVisual = wheels.leftFront.transform.Find("TankFree_Wheel_f_left");
        wheels.rightBackVisual = wheels.rightBack.transform.Find("TankFree_Wheel_b_right");
        wheels.rightFrontVisual = wheels.rightFront.transform.Find("TankFree_Wheel_f_right");

        // Initialize raycast origins
        raycastOrigins = new Transform[4];
        for (int i = 0; i < raycastOrigins.Length; i++)
        {
            raycastOrigins[i] = new GameObject($"RaycastOrigin_{i}").transform;
            raycastOrigins[i].parent = playerTank.transform;  // Set to the parent transform
        }

        raycastOrigins[0].position = wheels.leftFront.position;
        raycastOrigins[1].position = wheels.rightFront.position;
        raycastOrigins[2].position = wheels.leftBack.position;
        raycastOrigins[3].position = wheels.rightBack.position;
    }

    public void ResetForRespawn()
    {
        maxSpeed = data.maxSpeed;
    }

    public void TotalReset()
    {
        maxSpeed = data.maxSpeed;
    }

    public void Root()
    {
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Unroot()
    {
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    public void ChangeMaxSpeed(float modifier)
    {
        Debug.Log("Adding speed modifier: " + modifier);
        maxSpeed += modifier;
        EventManager.TriggerSpeedChanged(owner, maxSpeed, data.maxSpeed);
    }

    private void CalculateWeightTransfer(float verticalInput, float horizontalInput)
    {
        // Handle vertical input
        if (Mathf.Abs(lastVerticalInput - verticalInput) > 0.01f)
        {
            verticalWeightTransferResetTimer = 0f;
        }
        else
        {
            verticalWeightTransferResetTimer += Time.fixedDeltaTime;
        }

        // Handle horizontal input
        if (Mathf.Abs(lastHorizontalInput - horizontalInput) > 0.01f)
        {
            horizontalWeightTransferResetTimer = 0f;
        }
        else
        {
            horizontalWeightTransferResetTimer += Time.fixedDeltaTime;
        }

        lastVerticalInput = verticalInput;
        lastHorizontalInput = horizontalInput;

        // Calculate lerp factors
        float verticalLerpFactor = Mathf.Clamp01(verticalWeightTransferResetTimer / data.weightTransferResetTime);
        float horizontalLerpFactor = Mathf.Clamp01(horizontalWeightTransferResetTimer / data.weightTransferResetTime);

        float targetFrontForce = verticalInput > 0 ? data.weightTransferAmount : (verticalInput < 0 ? -data.weightTransferAmount : 0);
        float targetRightForce = horizontalInput < 0 ? -data.weightTransferAmount : (horizontalInput > 0 ? data.weightTransferAmount : 0);

        // Gradually reset weight transfer to balanced state
        targetFrontForce *= 1 - verticalLerpFactor;
        targetRightForce *= 1 - horizontalLerpFactor;

        frontSuspensionForce = Mathf.Lerp(frontSuspensionForce, targetFrontForce, Time.fixedDeltaTime * data.weightTransferSpeed);
        backSuspensionForce = -frontSuspensionForce;

        rightSuspensionForce = Mathf.Lerp(rightSuspensionForce, targetRightForce, Time.fixedDeltaTime * data.weightTransferSpeed);
        leftSuspensionForce = -rightSuspensionForce;
    }

    private void SimulateSuspension(Transform wheel, Transform raycastOrigin, float weightTransferForce)
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, -wheel.up, out hit, data.restDistance + 0.5f))
        {
            Vector3 normal = hit.normal;
            float distance = hit.distance;
            float difference = data.restDistance - distance;

            // Spring force
            float springForce = difference * data.suspensionForce;

            // Damping force
            float damper = -data.suspensionDamp * rb.GetPointVelocity(wheel.position).y;

            // Applying forces
            float totalForce = springForce + damper + weightTransferForce;

            // Adjust the force application direction based on the hit surface normal
            Vector3 force = normal * totalForce;
            rb.AddForceAtPosition(force, wheel.position);

            // Drawing force vector as an arrow for visualization
            Debug.DrawRay(wheel.position, force.normalized * 2f, Color.green);
            Debug.DrawRay(wheel.position + force.normalized * 2f, Quaternion.Euler(0, 30, 0) * -force.normalized * 0.5f, Color.green);
            Debug.DrawRay(wheel.position + force.normalized * 2f, Quaternion.Euler(0, -30, 0) * -force.normalized * 0.5f, Color.green);

            // Update the visual wheel position without affecting the raycast origin
            float adjustedRestDistance = data.restDistance - hit.distance;
            Vector3 newWheelPosition = raycastOrigin.position + raycastOrigin.up * adjustedRestDistance;
            wheel.position = newWheelPosition;
        }
    }


    public void Move(Transform transform, float verticalInput, float horizontalInput)
    {
        CalculateWeightTransfer(verticalInput, horizontalInput);

        SimulateSuspension(wheels.leftFront, raycastOrigins[0], frontSuspensionForce + leftSuspensionForce);
        SimulateSuspension(wheels.rightFront, raycastOrigins[1], frontSuspensionForce + rightSuspensionForce);
        SimulateSuspension(wheels.leftBack, raycastOrigins[2], backSuspensionForce + leftSuspensionForce);
        SimulateSuspension(wheels.rightBack, raycastOrigins[3], backSuspensionForce + rightSuspensionForce);

        // Check if the tank is grounded
        bool isGrounded = Physics.Raycast(wheels.leftFront.position, -wheels.leftFront.up, data.restDistance + 0.5f) ||
                        Physics.Raycast(wheels.rightFront.position, -wheels.rightFront.up, data.restDistance + 0.5f) ||
                        Physics.Raycast(wheels.leftBack.position, -wheels.leftBack.up, data.restDistance + 0.5f) ||
                        Physics.Raycast(wheels.rightBack.position, -wheels.rightBack.up, data.restDistance + 0.5f);

        Vector3 currentVelocity = rb.velocity;
        currentVelocity.y = 0; // Remove the vertical component

        // Counteract the drift
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(rb.velocity, transform.forward);
        Vector3 rightVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right);
        rb.velocity = forwardVelocity + rightVelocity * 0.1f; // You can tune the 0.1f value

        // Apply movement
        if (verticalInput != 0 && currentVelocity.magnitude < maxSpeed)
        {
            Vector3 force = transform.forward * verticalInput * data.movementAcceleration * Time.fixedDeltaTime;
            rb.AddForce(force, ForceMode.Impulse);
        }
        else if (isGrounded && verticalInput == 0)
        {
            // Apply a strong damping force when not moving to make it stop quickly
            Vector3 dampingForce = -rb.velocity * data.movementAcceleration * Time.fixedDeltaTime;
            rb.AddForce(dampingForce, ForceMode.Impulse);
        }

        // Apply rotation
        if (horizontalInput != 0)
        {
            float rotationSpeed = horizontalInput * data.rotationSpeed;
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, rotationSpeed, rb.angularVelocity.z);
        }
        else if (isGrounded)
        {
            // Stop rotation when there is no input
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, 0, rb.angularVelocity.z);
        }

        RotateWheels(verticalInput, horizontalInput);
    }

    private void RotateWheels(float verticalInput, float horizontalInput)
    {
        float wheelRotationAmount = 0;

        if (verticalInput != 0)
        {
            // Move forward or backward
            wheelRotationAmount = verticalInput * data.wheelRotationSpeed * Time.fixedDeltaTime;
        }

        if (horizontalInput != 0)
        {
            // Rotating the tank
            wheelRotationAmount += horizontalInput * data.wheelRotationSpeed * Time.fixedDeltaTime / 2.0f;
            float leftWheelRotation = wheelRotationAmount;
            float rightWheelRotation = -wheelRotationAmount;

            wheels.leftFrontVisual.Rotate(Vector3.right, leftWheelRotation);
            wheels.rightFrontVisual.Rotate(Vector3.right, rightWheelRotation);
            wheels.leftBackVisual.Rotate(Vector3.right, leftWheelRotation);
            wheels.rightBackVisual.Rotate(Vector3.right, rightWheelRotation);
        }
        else
        {
            // Simply move forward or backward
            wheels.leftFrontVisual.Rotate(Vector3.right, wheelRotationAmount);
            wheels.rightFrontVisual.Rotate(Vector3.right, wheelRotationAmount);
            wheels.leftBackVisual.Rotate(Vector3.right, wheelRotationAmount);
            wheels.rightBackVisual.Rotate(Vector3.right, wheelRotationAmount);
        }
    }
}
