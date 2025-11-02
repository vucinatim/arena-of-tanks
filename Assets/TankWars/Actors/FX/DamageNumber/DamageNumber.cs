using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public TextMeshPro textMeshPro; // Reference to the TextMeshPro component
    public AnimationCurve animationCurve; // Animation curve for all parameters
    public float lifeDuration = 2f; // Duration in seconds for the damage number to stay visible
    public float minScale = 0.9f; // Minimum scale of the damage number text
    public float maxScale = 1.3f; // Maximum scale of the damage number text
    public float moveMulti = 4f; // Speed at which the damage number moves
    public Gradient colorGradient; // Gradient for coloring the damage number text based on value

    private float startTime; // Time at which the damage number was instantiated
    private Vector3 moveDirection; // Direction of movement for the damage number

    void Awake()
    {
        // Get the TextMeshPro component from the same game object
        textMeshPro = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        startTime = Time.time; // Record the start time of the damage number

        // Generate random direction and speed for movement
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        float curveValue = animationCurve.Evaluate(elapsedTime / lifeDuration);

        // Move the damage number based on the moveDirection and moveSpeed
        float moveSpeed = curveValue * moveMulti;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Update the size of the damage number based on the damageValue
        float damageValue = int.Parse(textMeshPro.text);
        float scaleValue = Mathf.Lerp(minScale, maxScale, damageValue / 100f * curveValue);
        textMeshPro.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);

        // Update the color of the damage number based on the damageValue and colorGradient
        textMeshPro.color = colorGradient.Evaluate(damageValue / 100f);

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

    public void UpdateDamageNumber(int damageValue)
    {
        // Update the text of the damage number
        textMeshPro.text = damageValue.ToString();
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
