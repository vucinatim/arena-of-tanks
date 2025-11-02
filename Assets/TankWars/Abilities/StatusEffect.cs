using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    Stun,
    Slow,
    Poison,
    Burn,
    // Add more status effect types here
}

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Status Effects/Status Effect")]
public class StatusEffect : ScriptableObject
{
    public StatusEffectType type; // Type of the status effect
    public Sprite icon; // Icon to display in the UI
    public float duration; // Duration of the status effect
    public float effectValue; // Value of the effect (e.g. stun duration, slow amount, poison damage)
    public float updatePeriod; // How often to update the status effect (e.g. for damage over time effects)

    public GameObject fxObject; // The particle effect to play when the status effect is applied
    public GameObject textPrefab; // The text prefab to display when the status effect is applied

    private float timer; // Timer to track when to update the status effect

    public void ApplyStatusEffect(GameObject applier, GameObject target)
    {
        switch (type)
        {
            case StatusEffectType.Stun:
                target.transform.GetComponent<PhysicsMovementSystem>().Root();
                break;
            case StatusEffectType.Slow:
                target.transform.GetComponent<PhysicsMovementSystem>().ChangeMaxSpeed(effectValue);
                break;
            case StatusEffectType.Poison:
                target.transform.GetComponent<PhysicsMovementSystem>().ChangeMaxSpeed(-effectValue);
                break;
            case StatusEffectType.Burn:
                break;
            default:
                break;
        }
    }

    public void UpdateStatusEffect(GameObject applier, GameObject target)
    {
        switch (type)
        {
            case StatusEffectType.Stun:
                break;
            case StatusEffectType.Slow:
                break;
            case StatusEffectType.Poison:
                ApplyDamageOverTime(applier, target, effectValue, updatePeriod, Time.deltaTime);
                break;
            case StatusEffectType.Burn:
                ApplyDamageOverTime(applier, target, effectValue, updatePeriod, Time.deltaTime);
                break;
            default:
                break;
        }
    }

    public void RemoveStatusEffect(GameObject applier, GameObject target)
    {
        switch (type)
        {
            case StatusEffectType.Stun:
                target.transform.GetComponent<PhysicsMovementSystem>().Unroot();
                break;
            case StatusEffectType.Slow:
                target.transform.GetComponent<PhysicsMovementSystem>().ChangeMaxSpeed(effectValue);
                break;
            case StatusEffectType.Poison:
                target.transform.GetComponent<PhysicsMovementSystem>().ChangeMaxSpeed(effectValue);
                break;
            case StatusEffectType.Burn:
                break;
            default:
                break;
        }
    }

    private void ApplyDamageOverTime(GameObject applier, GameObject target, float damagePerSecond, float interval, float deltaTime)
    {
        var healthSystem = target.GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            return;
        }
        timer += deltaTime;
        if (timer >= interval)
        {
            healthSystem.ApplyDamage(applier, damagePerSecond * interval);
            timer = 0f; // Reset damage timer
        }
    }

}
