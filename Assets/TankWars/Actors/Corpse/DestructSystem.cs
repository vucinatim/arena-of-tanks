using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DestructData
{
    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    public float explosionForce;
    public float explosionRadius;
}

public class DestructSystem : MonoBehaviour
{
    [SerializeField] private DestructData data;

    private class DestructiblePartData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    private Dictionary<GameObject, DestructiblePartData> parts = new Dictionary<GameObject, DestructiblePartData>();

    public Action OnDestruct;

    public void Initialize(DestructData destructData)
    {
        data = destructData;
    }

    public void Reset()
    {
        foreach (KeyValuePair<GameObject, DestructiblePartData> part in parts)
        {
            part.Key.transform.transform.position = part.Value.position;
            part.Key.transform.transform.rotation = part.Value.rotation;
        }
    }

    public void SelfDestruct()
    {
        if (data.explosionPrefab != null)
        {
            GameObject explosion = Instantiate(data.explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        if (data.explosionSound != null)
        {
            AudioManager.Instance.PlaySFX(data.explosionSound);
        }

        // Apply explosion force to each destructible child of the current game object
        foreach (Transform childTransform in transform)
        {
            GameObject child = childTransform.gameObject;
            if (child.CompareTag("Destructible"))
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Store original position and rotation
                    parts.Add(child, new DestructiblePartData { position = child.transform.position, rotation = child.transform.rotation });

                    // Randomize explosion center within a small radius around the parent transform
                    Vector3 explosionCenterOffset = UnityEngine.Random.insideUnitSphere * 0.5f; // adjust 0.5f to change the variability range
                    Vector3 explosionCenter = transform.position + explosionCenterOffset;

                    // Apply explosion force with some random variation
                    float randomizedExplosionForce = data.explosionForce + UnityEngine.Random.Range(-0.1f * data.explosionForce, 0.1f * data.explosionForce); // adjust range as needed
                    rb.AddExplosionForce(randomizedExplosionForce, explosionCenter, data.explosionRadius);

                    // Apply a random torque
                    rb.AddTorque(UnityEngine.Random.insideUnitSphere * randomizedExplosionForce, ForceMode.Impulse);
                }
            }
        }
    }

}