using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffectsData
{
    public bool isEffectable = true;
}

public class StatusEffectsSystem : MonoBehaviour
{
    private Player owner;

    [SerializeField] private StatusEffectsData data;

    private bool isEffectable = true;
    // Class to hold information about an active status effect instance
    public class ActiveStatusEffect
    {
        public StatusEffect statusEffect;
        public float remainingDuration;
        public GameObject fxObject;
        public GameObject applier;
    }

    List<ActiveStatusEffect> activeStatusEffects = new List<ActiveStatusEffect>(); // List to store active status effects

    public void Initialize(Player owner, StatusEffectsData data)
    {
        this.owner = owner;
        this.data = data;

        isEffectable = data.isEffectable;
    }

    public void ResetForRespawn()
    {
        isEffectable = data.isEffectable;
        for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
        {
            DeactivateAndRemoveStatusEffect(activeStatusEffects[i]);
        }
    }

    public void TotalReset()
    {
        isEffectable = data.isEffectable;
        for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
        {
            DeactivateAndRemoveStatusEffect(activeStatusEffects[i]);
        }
    }

    // Add a status effect to the player
    public void AddStatusEffect(GameObject applier, StatusEffect statusEffect)
    {
        // Check if an instance of the status effect is already active
        var existingStatusEffect = activeStatusEffects.Find(x => x.statusEffect == statusEffect);

        if (existingStatusEffect != null)
        {
            // If an instance is already active, reset its duration
            existingStatusEffect.remainingDuration = statusEffect.duration;
        }
        else
        {
            // If no instance is active, create a new instance and add it to the list
            var newActiveStatusEffect = new ActiveStatusEffect
            {
                statusEffect = statusEffect,
                remainingDuration = statusEffect.duration,
                applier = applier,
                fxObject = FXManager.Instance.SpawnFX(statusEffect.fxObject, transform.position, transform.rotation, owner.transform)
            };

            activeStatusEffects.Add(newActiveStatusEffect);

            statusEffect.ApplyStatusEffect(newActiveStatusEffect.applier, gameObject);

            EventManager.TriggerStatusEffectAdded(owner, statusEffect);

            // Spawn a particle effect for the status effect
            if (statusEffect.textPrefab != null)
            {
                FXManager.Instance.SpawnFX(statusEffect.textPrefab, transform.position, transform.rotation, owner.transform);
            }
        }
    }

    // Remove a status effect from the player
    public void DeactivateAndRemoveStatusEffect(ActiveStatusEffect activeStatusEffect)
    {

        if (activeStatusEffect != null)
        {
            activeStatusEffect.statusEffect.RemoveStatusEffect(activeStatusEffect.applier, gameObject);
            // Remove the instance from the list and destroy its FX object
            activeStatusEffects.Remove(activeStatusEffect);
            FXManager.Instance.RemoveFX(activeStatusEffect.fxObject.GetInstanceID());

            EventManager.TriggerStatusEffectRemoved(owner, activeStatusEffect.statusEffect);
        }
    }

    // Update the remaining duration of active status effects and remove expired effects
    private void Update()
    {
        // Loop through all active status effects
        for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
        {
            var activeStatusEffect = activeStatusEffects[i];

            // Decrement the remaining duration of the effect
            activeStatusEffect.remainingDuration -= Time.deltaTime;

            if (activeStatusEffect.remainingDuration <= 0)
            {
                // If the effect has expired, deactivate and remove it
                DeactivateAndRemoveStatusEffect(activeStatusEffect);
            }
            else
            {
                activeStatusEffect.statusEffect.UpdateStatusEffect(activeStatusEffect.applier, gameObject);
            }
        }
    }

    // Check if the player has a specific status effect active
    public bool HasStatusEffect(StatusEffectType type)
    {
        foreach (var activeStatusEffect in activeStatusEffects)
        {
            if (activeStatusEffect.statusEffect.type == type)
            {
                return true;
            }
        }
        return false;
    }

    // Get the value of a specific status effect
    public float GetStatusEffectValue(StatusEffectType type)
    {
        foreach (var activeStatusEffect in activeStatusEffects)
        {
            if (activeStatusEffect.statusEffect.type == type)
            {
                return activeStatusEffect.statusEffect.effectValue;
            }
        }
        return 0;
    }
}
