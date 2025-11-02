using UnityEngine;

[CreateAssetMenu]
public class TurretAbility : Ability
{
    public GameObject bulletPrefab;
    public GameObject bulletHitFXPrefab;
    public float bulletSpeed = 20.0f;
    public float fireRate = 0.2f;
    public float bulletDamage = 10.0f;
    public AudioClip fireSound;
    public AudioClip hitSound;

    private GameObject parent;
    private Transform shootingPoint;
    private float fireTimer = 0.0f;

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);
        this.parent = parent;

        shootingPoint = parent.transform.Find("ShootingPoint");
        fireTimer = 0.0f;
    }

    public override void update(GameObject parent)
    {
        base.update(parent);

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            FireBullet();
            fireTimer = 0.0f;
        }
    }

    private void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = shootingPoint.forward * bulletSpeed;
        
        // Add your collision handling code here similar to MissileAbility.
        bullet.GetComponent<CollisionSystem>().OnCollision += HandleBulletCollision;
        AudioManager.Instance.PlaySFX(fireSound);
    }

    private void HandleBulletCollision(GameObject bullet, GameObject other)
    {
        Debug.Log("Bullet collided with " + other.name);
        bullet.GetComponent<CollisionSystem>().OnCollision -= HandleBulletCollision;

        // Play hit sound
        if (hitSound != null)
        {
            AudioManager.Instance.PlaySFX(hitSound);
        }

        // Other collision logic like damage, knockback, etc.
        // Check if the collider has a DamageHandler script attached
        HealthSystem damageHandler = other.GetComponentInParent<HealthSystem>();
        if (damageHandler != null)
        {
            // Calculate damage based on blast radius, projectile speed, etc.
            damageHandler.ApplyDamage(parent, bulletDamage);
        }

        // Check if the collider is explodable
        IExplodable explodable = other.GetComponentInParent<IExplodable>();
        explodable?.Explode();

        // Spawn bullet hit FX
        FXManager.Instance.SpawnFX(bulletHitFXPrefab, bullet.transform.position, bullet.transform.rotation, null, 1f);
        
        // Destroy the bullet
        Destroy(bullet);
    }

    public override void Deactivate(GameObject parent)
    {
        base.Deactivate(parent);
        // Any additional cleanup, if needed.
    }
}
