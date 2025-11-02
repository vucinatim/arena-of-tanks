using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AbilityState
{
    inactive,
    active,
}

public class Ability : PickupableData
{
    public new string name;
    public Sprite abilityIcon;
    public AudioClip activateSound;
    public AudioClip activeSound;
    public AudioClip deactivateSound;
    public float activeTime;
    private AbilityState state = AbilityState.inactive;

    private AudioSource activeAudioSource;

    internal AbilityState State { get => state; set => state = value; }

    public virtual void Activate(GameObject parent) {
        State = AbilityState.active;
        if (activateSound != null)
        {
            AudioManager.Instance.PlaySFX(activateSound);
        }
        if (activeSound != null)
        {
            activeAudioSource = AudioManager.Instance.PlayAndLoop(activeSound);
        }
     }
    public virtual void Deactivate(GameObject parent) { 
        State = AbilityState.inactive;
        if (activeAudioSource != null)
        {
            Destroy(activeAudioSource.gameObject);
        }
        if (deactivateSound != null)
        {
            AudioManager.Instance.PlaySFX(deactivateSound);
        }
    }

    public virtual void update(GameObject parent) {
        
    }

    public override bool OnCollected(GameObject collector)
    {
        if (collector.TryGetComponent<AbilityQueueSystem>(out var system))
        {
            system.EnqueueAbility(this);
            return true;
        }

        return false;
    }
}
