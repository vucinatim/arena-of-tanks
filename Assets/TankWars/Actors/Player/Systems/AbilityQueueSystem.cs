using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class AbilityQueueData
{
    public int abilityCount;
    public List<Ability> abilities;
}

public class AbilityQueueSystem : MonoBehaviour
{
    private Player owner;
    [SerializeField] private AbilityQueueData data;
    [SerializeField] private Queue<Ability> abilitiesQueue = new Queue<Ability>();

    private Ability activeAbility;
    private float activeTime;

    public void Initialize(Player owner, AbilityQueueData data)
    {
        this.owner = owner;
        this.data = data;

        activeAbility = null;
        activeTime = 0;
        abilitiesQueue.Clear();
        foreach (var ability in this.data.abilities)
        {
            EnqueueAbility(ability);
        }
    }

    public void ResetForRespawn()
    {
        activeAbility?.Deactivate(gameObject);
        activeAbility = null;
        activeTime = 0;
        foreach (var ability in abilitiesQueue)
        {
            EventManager.TriggerAbilityDequeued(owner, ability);
        }
        GetComponent<OverheadDisplaySystem>().SetCurrentAbilityIcon(null);
        abilitiesQueue.Clear();
    }

    public void TotalReset()
    {
        activeAbility?.Deactivate(gameObject);
        activeAbility = null;
        activeTime = 0;
        foreach (var ability in abilitiesQueue)
        {
            EventManager.TriggerAbilityDequeued(owner, ability);
        }
        GetComponent<OverheadDisplaySystem>().SetCurrentAbilityIcon(null);
        abilitiesQueue.Clear();
        foreach (var ability in data.abilities)
        {
            EnqueueAbility(ability);
        }
    }

    public bool IsEmpty()
    {
        if (abilitiesQueue.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetMaxAbilitiesCount()
    {
        return data.abilityCount;
    }

    public void EnqueueAbility(Ability ability)
    {
        abilitiesQueue.Enqueue(ability);
        if (abilitiesQueue.Count > data.abilityCount)
        {
            DequeueAbility();
        }

        EventManager.TriggerAbilityQueued(owner, ability);
        if (abilitiesQueue.Count == 1)
        {
            GetComponent<OverheadDisplaySystem>().SetCurrentAbilityIcon(ability.abilityIcon);
            EventManager.TriggerCurrentAbilityChanged(owner, ability);
        }
    }

    public Ability DequeueAbility()
    {
        var removedAbility = abilitiesQueue.Dequeue();

        EventManager.TriggerAbilityDequeued(owner, removedAbility);
        if (abilitiesQueue.Count > 0)
        {
            GetComponent<OverheadDisplaySystem>().SetCurrentAbilityIcon(PeekAbility().abilityIcon);
            EventManager.TriggerCurrentAbilityChanged(owner, PeekAbility());
        }
        else
        {
            GetComponent<OverheadDisplaySystem>().SetCurrentAbilityIcon(null);
            EventManager.TriggerCurrentAbilityChanged(owner, null);
        }
        return removedAbility;
    }

    public Ability PeekAbility()
    {
        return abilitiesQueue.Peek();
    }

    public void TriggerAbility()
    {
        if (IsEmpty())
        {
            return;
        }

        if (!activeAbility)
        {
            activeAbility = PeekAbility();
            activeTime = activeAbility.activeTime;
            activeAbility.Activate(gameObject);
            EventManager.TriggerAbilityActivated(owner, activeAbility);
            GetComponent<OverheadDisplaySystem>().TriggerCurrentAbility(activeAbility.activeTime);
        }
        else
        {
            EventManager.TriggerAbilityDeactivated(owner, activeAbility);
            activeAbility.Deactivate(gameObject);
            activeAbility = null;
            activeTime = 0;
            DequeueAbility();
        }
    }

    void Update()
    {
        if (!activeAbility)
        {
            return;
        }

        if (activeTime > 0 && activeAbility.State == AbilityState.active)
        {
            activeTime -= Time.deltaTime;
            activeAbility.update(gameObject);
        }
        else
        {
            DequeueAbility();
            EventManager.TriggerAbilityDeactivated(owner, activeAbility);
            activeAbility.Deactivate(gameObject);
            activeAbility = null;
            activeTime = 0;
        }
    }
}
