using UnityEngine;

[CreateAssetMenu]
public class StunAbility : Ability
{
    public StatusEffect statusEffect;

    public GameObject projectilePrefab;
    public GameObject fxPrefab;
    public float projectileSpeed = 10.0f;

    public float blastRadius = 10.0f;

    private GameObject spawnedProjectile;
    private GameObject spawnedFX;

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);

        var shootingPoint = parent.transform.Find("ShootingPoint");
        spawnedProjectile = Instantiate(projectilePrefab, shootingPoint.transform.position, shootingPoint.transform.rotation * projectilePrefab.transform.rotation);
        spawnedProjectile.GetComponent<Rigidbody>().velocity = shootingPoint.transform.forward * projectileSpeed;
    }

    public override void Deactivate(GameObject parent)
    {
        var blastPoint = spawnedProjectile.transform;
        FXManager.Instance.SpawnFX(fxPrefab, blastPoint.position, blastPoint.rotation, null, 1f);
        Destroy(spawnedProjectile);

        // Show blast radius sphere
        DebugManager.Instance.ShowBlastRadiusSphere(blastPoint.position, blastRadius, 3f, Color.blue);

        // Apply status effect to affected players
        Collider[] colliders = Physics.OverlapSphere(blastPoint.position, blastRadius);
        foreach (Collider collider in colliders)
        {
            // Check if the collider has a StatusEffectsSystem script attached
            StatusEffectsSystem statusEffectsSystem = collider.GetComponentInParent<StatusEffectsSystem>();
            if (statusEffectsSystem != null)
            {
                // Add SE.
                statusEffectsSystem.AddStatusEffect(parent, statusEffect);
            }
        }

    }
}
