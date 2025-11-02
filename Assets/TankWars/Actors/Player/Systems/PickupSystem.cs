using System;
using System.Collections.Generic;
using UnityEngine;
public class PickupSystem : MonoBehaviour
{
    private Player owner;
    public List<PickupableData> collectedPickupables = new List<PickupableData>();

    public void Initialize(Player owner)
    {
        this.owner = owner;
    }
    public void ResetForRespawn()
    {
        // collectedPickupables.Clear();
    }
    public void TotalReset()
    {
        collectedPickupables.Clear();
    }

    public bool Pickup(PickupableData pickupable)
    {
        if (pickupable == null)
        {
            Debug.LogError("Pickupable data is null");
            return false;
        }

        var wasCollected = pickupable.OnCollected(gameObject);

        if (wasCollected)
        {
            collectedPickupables.Add(pickupable);
            FXManager.Instance.SpawnFX("PickupText", transform.position, Quaternion.identity, owner.transform, 2f, pickupable.pickupableName, pickupable.pickupableColor ?? Constants.rarityColors[pickupable.rarityType]);
            AudioManager.Instance.PlaySFX("PickupCollected");
            EventManager.TriggerPickupCollected(owner, pickupable);
            return true;
        }
        else
        {
            return false;
        }
    }
}
