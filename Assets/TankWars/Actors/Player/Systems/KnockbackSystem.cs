using UnityEngine;
using System;

public class KnockbackSystem : MonoBehaviour
{
    private Rigidbody rb;
    private Player owner;

    private float knockbackMultiplier = 1f;

    public void Initialize(Player owner)
    {
        this.owner = owner;
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyKnockback(GameObject source, Vector3 direction, float force)
    {
        Vector3 knockback = direction.normalized * force * knockbackMultiplier;
        rb.AddForce(knockback, ForceMode.Impulse);

        EventManager.TriggerKnockbackTaken(owner, source, knockback);
    }

    public void SetKnockbackMultiplier(float multiplier)
    {
        knockbackMultiplier = multiplier;
    }
}
