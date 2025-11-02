using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class AISystem : MonoBehaviour
{
    private Player player;
    private ControlSystem controlSystem;
    private NavMeshPath path;
    private int currentWaypointIndex;
    private float waypointThreshold = 2f; // Distance to consider a waypoint reached

    private GameObject[] waypointMarkers; // Array to hold markers for cleanup

    private float moveUpdateInterval = 0.1f; // Interval in seconds (200ms)
    private float lastMoveUpdateTime = 0f; // Tracks the last update time

    void Awake()
    {
        player = GetComponent<Player>();
        controlSystem = GetComponent<ControlSystem>();
        path = new NavMeshPath();
        currentWaypointIndex = 0;

        waypointMarkers = new GameObject[10]; // Max waypoints for visualization
    }

    void Start()
    {
        DecideNextAction();
    }

    void Update()
    {
        if (path.corners.Length == 0 || currentWaypointIndex >= path.corners.Length)
            return;

        // Check if we reached the current waypoint
        Vector3 waypoint = path.corners[currentWaypointIndex];
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoint);

        if (distanceToWaypoint < waypointThreshold)
        {
            Debug.Log($"Waypoint {currentWaypointIndex} reached.");
            currentWaypointIndex++;

            // If we've reached the final waypoint, decide the next action
            if (currentWaypointIndex >= path.corners.Length)
            {
                Debug.Log("Target reached, deciding next action.");
                DecideNextAction();
                return;
            }
        }

        // Continuously adjust movement towards the current waypoint
        MoveTowardsWaypoint(path.corners[currentWaypointIndex]);
    }

    private void MoveTowardsWaypoint(Vector3 waypoint)
    {
        // Throttle the function: Only execute if enough time has passed
        if (Time.time - lastMoveUpdateTime < moveUpdateInterval)
            return;

        lastMoveUpdateTime = Time.time; // Update the last execution time

        Vector3 toWaypoint = (waypoint - transform.position).normalized;

        // Calculate the angle between the tank's forward direction and the waypoint
        float angleToWaypoint = Vector3.SignedAngle(transform.forward, toWaypoint, Vector3.up);

        // Rotation smoothing: Use a dead zone to prevent rapid alternating
        if (Mathf.Abs(angleToWaypoint) > 10f) // Significant angle, adjust rotation
        {
            if (angleToWaypoint > 0)
            {
                controlSystem.ButtonInput("left-up"); // Stop counterclockwise
                controlSystem.ButtonInput("right"); // Rotate clockwise
            }
            else
            {
                controlSystem.ButtonInput("right-up"); // Stop clockwise
                controlSystem.ButtonInput("left"); // Rotate counterclockwise
            }
        }
        else // Stop rotating when within the dead zone
        {
            controlSystem.ButtonInput("right-up");
            controlSystem.ButtonInput("left-up");
        }

        // Allow forward movement when the angle is within a reasonable range
        if (Mathf.Abs(angleToWaypoint) < 45f) // Forward movement angle threshold
        {
            controlSystem.ButtonInput("up");
        }
        else
        {
            controlSystem.ButtonInput("up-up"); // Stop forward movement if angle is too large
        }
    }

    private void DecideNextAction()
    {
        // Find the nearest pickupable object
        GameObject nearestPickup = FindNearestPickupable();

        if (nearestPickup != null)
        {
            Debug.Log($"Nearest pickup found at: {nearestPickup.transform.position}");
            SetTarget(nearestPickup.transform.position);
        }
        else
        {
            Debug.Log("No pickups found, idling or setting random target.");
            SetRandomTarget();
            // controlSystem.ButtonInput("up-up"); // Stop movement
            // controlSystem.ButtonInput("left-up"); // Stop rotation
            // controlSystem.ButtonInput("right-up"); // Stop rotation
            // controlSystem.ButtonInput("left-up"); // Stop rotation
        }
    }

    private GameObject FindNearestPickupable()
    {
        GameObject[] pickupables = GameObject.FindGameObjectsWithTag("Pickupable");
        if (pickupables.Length == 0)
            return null;

        GameObject nearest = pickupables
            .OrderBy(p => Vector3.Distance(transform.position, p.transform.position))
            .FirstOrDefault();

        return nearest;
    }

    private void SetTarget(Vector3 targetPosition)
    {
        ClearWaypointMarkers(); // Remove old markers

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            Debug.Log($"Setting target at: {hit.position}");
            NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path);

            if (path.corners.Length > 0)
            {
                Debug.Log($"Path generated with {path.corners.Length} waypoints.");
                CreateWaypointMarkers();
                currentWaypointIndex = 0;
            }
            else
            {
                Debug.LogWarning("Path generated but contains no corners!");
            }
        }
        else
        {
            Debug.LogWarning("Failed to set target.");
        }
    }

    private void SetRandomTarget()
    {
        ClearWaypointMarkers(); // Remove old markers

        Vector3 randomPoint = GameUtility.GetRandomPointInNavMesh();
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            Debug.Log($"Setting random target at: {hit.position}");
            NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path);

            if (path.corners.Length > 0)
            {
                Debug.Log($"Path generated with {path.corners.Length} waypoints.");
                CreateWaypointMarkers();
                currentWaypointIndex = 0;
            }
            else
            {
                Debug.LogWarning("Path generated but contains no corners!");
            }
        }
        else
        {
            Debug.LogWarning("Failed to find a valid random point.");
        }
    }

    private void CreateWaypointMarkers()
    {
        for (int i = 0; i < path.corners.Length; i++)
        {
            if (i < waypointMarkers.Length)
            {
                // Create a sphere at the waypoint position
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.GetComponent<Collider>().enabled = false; // Disable collider for visibility
                sphere.transform.position = path.corners[i] + Vector3.up; // Offset for visibility
                sphere.transform.localScale = Vector3.one * 2f; // Scale for visibility

                // Color the sphere based on its role
                if (i == 0)
                {
                    sphere.GetComponent<Renderer>().material.color = Color.red; // First waypoint
                }
                else if (i == path.corners.Length - 1)
                {
                    sphere.GetComponent<Renderer>().material.color = Color.green; // Last waypoint
                }
                else
                {
                    sphere.GetComponent<Renderer>().material.color = Color.cyan; // Intermediate waypoints
                }

                // Store the marker for cleanup
                waypointMarkers[i] = sphere;
            }
        }
    }

    private void ClearWaypointMarkers()
    {
        foreach (var marker in waypointMarkers)
        {
            if (marker != null)
            {
                Destroy(marker);
            }
        }
    }
}
