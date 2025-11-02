using UnityEngine;

[CreateAssetMenu]
public class MissileAbility : Ability
{
    public GameObject projectilePrefab;
    public GameObject fxPrefab;
    public float projectileSpeed = 10.0f;

    public float blastRadius = 10.0f;
    public float minDamage = 10.0f;
    public float maxDamage = 30.0f;

    public float knockbackForce = 10.0f;

    private GameObject spawnedProjectile;

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);

        var shootingPoint = parent.transform.Find("ShootingPoint");
        spawnedProjectile = Instantiate(projectilePrefab, shootingPoint.transform.position, shootingPoint.transform.rotation);
        spawnedProjectile.GetComponent<Rigidbody>().velocity = shootingPoint.transform.forward * projectileSpeed;

        // Subscribe to the OnCollision event of the ProjectileCollisionHandler
        spawnedProjectile.GetComponent<CollisionSystem>().OnCollision += HandleProjectileCollision;
    }

    private void HandleProjectileCollision(GameObject projectile, GameObject other)
    {
        Debug.Log("Missile collided with " + other.name);
        // Unsubscribe from the OnCollision event of the ProjectileCollisionHandler
        projectile.GetComponent<CollisionSystem>().OnCollision -= HandleProjectileCollision;
        // Set the ability state to inactive so that AbilityQueueSystem can dequeue the ability
        State = AbilityState.inactive;
    }

    public override void Deactivate(GameObject parent)
    {
        base.Deactivate(parent);
        
        CameraShakeEvent.Shake(0.5f, 0.8f);
        var blastPoint = spawnedProjectile.transform;
        FXManager.Instance.SpawnFX(fxPrefab, blastPoint.position, blastPoint.rotation, null, 2f);
        Destroy(spawnedProjectile);

        // Show blast radius sphere
        DebugManager.Instance.ShowBlastRadiusSphere(blastPoint.position, blastRadius, 3f, Color.red);

        // Apply damage to affected players
        Collider[] colliders = Physics.OverlapSphere(blastPoint.position, blastRadius);
        foreach (Collider collider in colliders)
        {
            // Check if the collider has a DamageHandler script attached
            HealthSystem damageHandler = collider.GetComponentInParent<HealthSystem>();
            if (damageHandler != null)
            {
                // Calculate damage based on blast radius, projectile speed, etc.
                int damage = GameUtility.CalculateDamage(blastRadius, minDamage, maxDamage, collider.transform, blastPoint);
                damageHandler.ApplyDamage(parent, damage);
            }

            // Check if the collider has a KnockbackSystem script attached
            KnockbackSystem knockbackSystem = collider.GetComponentInParent<KnockbackSystem>();
            if (knockbackSystem != null)
            {
                knockbackSystem.ApplyKnockback(parent, collider.transform.position - blastPoint.position, knockbackForce);
            }

            // Check if the collider is explodable
            IExplodable explodable = collider.GetComponentInParent<IExplodable>();
            explodable?.Explode();
        }

    }
}
