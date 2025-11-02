using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeEvent
{
    public delegate void ShakeCameraDelegate(float duration, float magnitude);
    public static event ShakeCameraDelegate OnShakeCamera;

    public static void Shake(float duration, float magnitude)
    {
        OnShakeCamera?.Invoke(duration, magnitude);
    }
}

public class CameraController : MonoBehaviour
{
    [SerializeField] private float smoothTime = 0.5f; // Smooth time for camera movement
    [SerializeField] private float minCameraAngle = 70f; // Minimum camera angle
    [SerializeField] private float maxCameraAngle = 80f; // Maximum camera angle
    [SerializeField] private float minDistance = 60f; // Minimum distance from players
    [SerializeField] private float maxDistance = 120; // Maximum distance from players
    [SerializeField] private float minZOffect = 30f; // Minimum Z-offset
    [SerializeField] private float maxZOffect = 20f; // Maximum Z-offset

    public bool isDynamic = false; // Whether the camera is dynamic or static

    // private List<Transform> players; // List of all player transforms
    private new Camera camera; // Reference to the camera component
    private Vector3 velocity; // Velocity for camera movement

    private void OnEnable()
    {
        CameraShakeEvent.OnShakeCamera += ShakeCamera;
    }

    private void OnDisable()
    {
        CameraShakeEvent.OnShakeCamera -= ShakeCamera;
    }

    void Start()
    {
        // players = PlayerManager.Instance.GetPlayerTransforms();
        camera = GetComponent<Camera>();

        // Set initial position and rotation of the camera
        // Vector3 averagePosition = GetAveragePosition();
        // transform.position = new Vector3(averagePosition.x, maxDistance, averagePosition.z);
        // transform.rotation = Quaternion.Euler(maxCameraAngle, 0f, 0f);
    }

    void LateUpdate()
    {
        if (isDynamic)
        {
            Bounds playerBounds = GetPlayerBounds();
            float boundingBoxSize = Mathf.Max(playerBounds.size.x, playerBounds.size.z);

            // Calculate the half size of the bounding box diagonally
            float halfDiagonalSize = boundingBoxSize * 0.5f * Mathf.Sqrt(2);

            // Determine how high the camera needs to be to fully view the bounding box
            float requiredHeight = halfDiagonalSize / Mathf.Tan(0.5f * camera.fieldOfView * Mathf.Deg2Rad);

            // Clamp the required height between minDistance and maxDistance
            requiredHeight = Mathf.Max(requiredHeight, minDistance);

            // Get the average position of all players (center of bounding box)
            Vector3 averagePosition = playerBounds.center;

            // Calculate the normalized value for height
            float t = Mathf.InverseLerp(minDistance, maxDistance, requiredHeight);

            // Calculate dynamic Z-offset
            float dynamicZOffset = Mathf.Lerp(-maxZOffect, -minZOffect, t); 

            // Calculate target position
            Vector3 targetPosition = new Vector3(averagePosition.x, requiredHeight, averagePosition.z + dynamicZOffset);

            // Smoothly move camera to target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // Lerp angle based on height
            float angle = Mathf.Lerp(minCameraAngle, maxCameraAngle, t);

            // Calculate target rotation
            Quaternion targetRotation = Quaternion.Euler(angle, 0, 0);

            // Smoothly rotate camera to target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime);
        }
    }


    Bounds GetPlayerBounds()
    {
        var players = PlayerManager.Instance.GetPlayerTransforms();
        if (players.Count == 0) return new Bounds(Vector3.zero, Vector3.zero);

        Bounds bounds = new Bounds(players[0].position, Vector3.zero);
        for (int i = 1; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].position);
        }
        return bounds;
    }

    // Camera shake functionality
    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(CameraShakeCoroutine(duration, magnitude));
    }

    private IEnumerator CameraShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    public void MoveToPosition(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        StartCoroutine(MoveCameraCoroutine(targetPosition, targetRotation, duration));
    }

    private IEnumerator MoveCameraCoroutine(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        float startTime = Time.time;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    public void ChangeFOV(float targetFOV, float duration)
    {
        StartCoroutine(ChangeFOVCoroutine(targetFOV, duration));
    }

    private IEnumerator ChangeFOVCoroutine(float targetFOV, float duration)
    {
        float startTime = Time.time;
        float startFOV = GetComponent<Camera>().fieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        GetComponent<Camera>().fieldOfView = targetFOV;

    }
}