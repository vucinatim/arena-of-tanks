using System.Collections;
using UnityEngine;

interface IExplodable
{
    void Explode();
}

public class ExplosiveBarrel : MonoBehaviour, IExplodable
{
    public float explosionRadius = 10.0f;
    public float explosionForce = 700.0f;
    public float minDamage = 10.0f;
    public float maxDamage = 100.0f;
    public GameObject explosionEffect;  // Drag your Explosion Effect prefab here
    public AudioClip explosionSound;    // Drag your Explosion Sound here

    private bool hasExploded = false;

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Instantiate explosion effect
        GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f); // Remove the explosion effect after 5 seconds

        // Play explosion sound
        if (explosionSound != null)
        {
            AudioManager.Instance.PlaySFX(explosionSound);
        }

        // Apply explosion force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // Show blast radius sphere
        DebugManager.Instance.ShowBlastRadiusSphere(transform.position, explosionRadius, 3f, Color.red);

        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Check if the collider has a DamageHandler script attached
            HealthSystem damageHandler = nearbyObject.GetComponentInParent<HealthSystem>();
            if (damageHandler != null)
            {
                // Calculate damage based on blast radius, projectile speed, etc.
                int damage = GameUtility.CalculateDamage(explosionRadius, minDamage, maxDamage, nearbyObject.transform, transform);
                damageHandler.ApplyDamage(gameObject, damage);
            }

            // Check if the collider is explodable
            IExplodable explodable = nearbyObject.GetComponentInParent<IExplodable>();
            if (explodable != null)
            {
                // Start coroutine to explode the object after a delay
                GameManager.Instance.StartCoroutine(ExplodeCoroutine(explodable));
            }
        }

        // Destroy the barrel
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }

    // Coroutine to explode the object after a delay
    private IEnumerator ExplodeCoroutine(IExplodable explodable)
    {
        yield return new WaitForSeconds(0.1f);
        explodable.Explode();
    }
}