using UnityEngine;

[CreateAssetMenu]
public class ShieldAbility : Ability
{
    public GameObject fxPrefab;

    private GameObject spawnedFX;

    private IModifier damageModifier = new DamageMultiplierModifier(0f);

    public override void Activate(GameObject parent)
    {   
        base.Activate(parent);

        spawnedFX = FXManager.Instance.SpawnFX(fxPrefab, parent.transform.position, Quaternion.identity, parent.transform);
        parent.GetComponent<HealthSystem>().AddDamageModifier(damageModifier);
        parent.GetComponent<KnockbackSystem>().SetKnockbackMultiplier(0f);
    }

    public override void Deactivate(GameObject parent)
    {
        base.Deactivate(parent);
        
        parent.GetComponent<HealthSystem>().RemoveDamageModifier(damageModifier);
        parent.GetComponent<KnockbackSystem>().SetKnockbackMultiplier(1f);
        FXManager.Instance.RemoveFX(spawnedFX.GetInstanceID());
    }
}
