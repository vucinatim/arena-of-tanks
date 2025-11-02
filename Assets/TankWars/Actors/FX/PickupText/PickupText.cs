using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickupText : MonoBehaviour
{
    private TextMeshPro textMeshPro;
    public AnimationCurve animationCurve;
    public float lifeDuration = 2f;
    public float minScale = 0.9f;
    public float maxScale = 1.3f;
    public float moveMulti = 4f;
    public Color color;

    private float startTime;
    private Vector3 moveDirection;

    void Awake()
    {
        // Get the TextMeshPro component from the same game object
        textMeshPro = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        startTime = Time.time; // Record the start time of the damage number

        // Generate random direction and speed for movement
        moveDirection = new Vector3(0f, 0f, 1f).normalized;
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        float curveValue = animationCurve.Evaluate(elapsedTime / lifeDuration);

        // Move the damage number based on the moveDirection and moveSpeed
        float moveSpeed = curveValue * moveMulti;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Update the size of the damage number based on the damageValue
        float scaleValue = Mathf.Lerp(minScale, maxScale, curveValue);
        textMeshPro.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);

        // Fade out the damage number based on the fade curve
        float fadeValue = curveValue;
        textMeshPro.alpha = fadeValue;

        // Despawn the damage number after the life duration is reached
        if (elapsedTime >= lifeDuration)
        {
            Destroy(gameObject);
        }

        // Always face the camera
        LookAtCamera();
    }

    public void UpdateText(string text)
    {
        textMeshPro.text = text;
    }

    void LookAtCamera()
    {
        // Calculate the direction from the damage number to the camera
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;

        // Calculate the target rotation based on the direction to the camera
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        // Keep the y axis rotation unchanged
        targetRotation.eulerAngles = new Vector3(-targetRotation.eulerAngles.x, 0, -targetRotation.eulerAngles.z);

        // Apply the target rotation to the damage number's transform rotation
        transform.rotation = targetRotation;
    }
}
