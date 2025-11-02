using UnityEngine;

[CreateAssetMenu]
public class FlamethrowerAbility : Ability
{
    public GameObject flamePrefab;
    public float initialFlameSpeed = 5.0f;
    public float maxFlameSpeed = 20.0f;
    public float speedIncreasePerSecond = 15.0f;  // How much speed to add per second
    public float fireRate = 0.1f;

    private Transform shootingPoint;
    private float fireTimer = 0.0f;
    private float currentFlameSpeed;
    private float timeSinceActivation = 0.0f;  // Track the time since activation

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);

        shootingPoint = parent.transform.Find("ShootingPoint");
        fireTimer = 0.0f;
        currentFlameSpeed = initialFlameSpeed;
        timeSinceActivation = 0.0f;  // Reset the timer
    }

    public override void update(GameObject parent)
    {
        base.update(parent);

        fireTimer += Time.deltaTime;
        timeSinceActivation += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            FireFlame();
            fireTimer = 0.0f;
        }

        // Linearly increase the flame speed, capping it at maxFlameSpeed
        currentFlameSpeed = Mathf.Min(initialFlameSpeed + timeSinceActivation * speedIncreasePerSecond, maxFlameSpeed);
    }

    private void FireFlame()
    {
        GameObject flame = Instantiate(flamePrefab, shootingPoint.position, shootingPoint.rotation);
        flame.GetComponent<Rigidbody>().velocity = shootingPoint.forward * currentFlameSpeed;
        
        // The Flame script will handle the rest
    }

    public override void Deactivate(GameObject parent)
    {
        base.Deactivate(parent);
        currentFlameSpeed = initialFlameSpeed; // Reset the speed
    }
}
