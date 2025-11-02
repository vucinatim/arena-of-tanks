using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DashAbility : Ability
{
    public GameObject afterImagePrefab;
    public float dashSpeed = 50f;
    public float dashDuration = 0.2f;
    public float afterImageSpawnRate = 0.05f;
    public float afterImageLifetime = 0.2f;

    private readonly List<GameObject> afterImages = new();
    private Coroutine afterImageCoroutine;

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);
        parent.GetComponent<MonoBehaviour>().StartCoroutine(DashCoroutine(parent));
    }

    private IEnumerator DashCoroutine(GameObject parent)
    {
        Transform tankTransform = parent.transform;
        var rb = parent.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        Vector3 initialPosition = tankTransform.position;
        Vector3 direction = tankTransform.forward;
        Vector3 targetPosition = initialPosition + direction * dashSpeed;

        // Raycasting to check for obstacles
        if (Physics.Raycast(initialPosition, direction, out RaycastHit hit, dashSpeed))
        {
            targetPosition = hit.point;
        }

        afterImageCoroutine = parent.GetComponent<MonoBehaviour>().StartCoroutine(SpawnAfterImage(parent));

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            float t = (Time.time - startTime) / dashDuration;
            tankTransform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        parent.GetComponent<MonoBehaviour>().StopCoroutine(afterImageCoroutine);
        rb.constraints = RigidbodyConstraints.None;
        Deactivate(parent);
    }

    private IEnumerator SpawnAfterImage(GameObject parent)
    {
        while (true)
        {
            GameObject afterImage = Instantiate(afterImagePrefab, parent.transform.position, parent.transform.rotation);
            afterImages.Add(afterImage);
            parent.GetComponent<MonoBehaviour>().StartCoroutine(DestroyAfter(afterImage, afterImageLifetime));
            yield return new WaitForSeconds(afterImageSpawnRate);
        }
    }

    private IEnumerator DestroyAfter(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(obj);
        afterImages.Remove(obj);
    }

    public override void Deactivate(GameObject parent)
    {
        base.Deactivate(parent);
    }
}
