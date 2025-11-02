using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class HealthData
{
    public float health;
}

public class HealthSystem : MonoBehaviour
{
    private Player owner;
    [SerializeField] private HealthData data;

    private float health;

    private List<IModifier> damageModifiers = new List<IModifier>();
    private List<IModifier> healingModifiers = new List<IModifier>();

    public void Initialize(Player owner, HealthData data)
    {
        this.owner = owner;
        this.data = data;
        SetHealth(data.health);
    }

    public void ResetForRespawn()
    {
        damageModifiers.Clear();
        healingModifiers.Clear();
        SetHealth(data.health);
    }

    public void TotalReset()
    {
        damageModifiers.Clear();
        healingModifiers.Clear();
        SetHealth(data.health);
    }

    public void SetHealth(float newHealth)
    {
        health = Math.Max(0, newHealth);
        EventManager.TriggerHealthChanged(owner, health, data.health);
    }

    public float ApplyDamage(GameObject damageDealer, float damage)
    {
        // Apply damage modifiers
        float actualDamage = damage;
        foreach (IModifier modifier in damageModifiers)
        {
            actualDamage = modifier.Modify(actualDamage);
        }

        SetHealth(health - actualDamage);

        EventManager.TriggerDamageTaken(owner, damageDealer, actualDamage);
        if (actualDamage > 0)
        {
            FXManager.Instance.SpawnFX("DamageNumber", transform.position, Quaternion.identity, null, 2f, actualDamage.ToString());
        }
        else
        {
            FXManager.Instance.SpawnFX("Block", transform.position, Quaternion.identity, owner.transform, 2f);
        }

        // Check if player health is zero or less
        if (health <= 0)
        {
            // Player is dead
            health = 0;
            EventManager.TriggerPlayerDeath(owner, damageDealer);

            // Subtract a life from the player
            transform.GetComponent<LivesSystem>().TakeALife(damageDealer);
        }

        return actualDamage;
    }

    public float ApplyHealing(GameObject healer, float healing)
    {
        // Apply healing modifiers
        float actualHealing = healing;
        foreach (IModifier modifier in healingModifiers)
        {
            actualHealing = modifier.Modify(actualHealing);
        }

        SetHealth(health + actualHealing);

        EventManager.TriggerHealingTaken(owner, healer, actualHealing);
        FXManager.Instance.SpawnFX("HealingNumber", transform.position, Quaternion.identity, owner.transform, 2f, healing.ToString());

        // Check if player health is greater than max health
        if (health > data.health)
        {
            health = data.health;
        }

        return actualHealing;
    }

    public void AddDamageModifier(IModifier modifier)
    {
        damageModifiers.Add(modifier);
    }

    public void RemoveDamageModifier(IModifier modifier)
    {
        damageModifiers.Remove(modifier);
    }

    public void AddHealingModifier(IModifier modifier)
    {
        healingModifiers.Add(modifier);
    }

    public void RemoveHealingModifier(IModifier modifier)
    {
        healingModifiers.Remove(modifier);
    }
}

