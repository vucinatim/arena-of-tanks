using System.Collections.Generic;
using UnityEngine;

class Pickupable : MonoBehaviour
{
    public PickupableData pickupableData;

    public void SetPickupable(PickupableData pickupableData)
    {
        this.pickupableData = pickupableData;
        var pickupableRenderer = GetComponentInChildren<Renderer>();
        if (pickupableRenderer == null)
        {
            Debug.LogError("Pickupable renderer is null");
            return;
        }
        pickupableRenderer.material.color = pickupableData.pickupableColor ?? Constants.rarityColors[pickupableData.rarityType];
    }

    // Triggered when this object collides with another object
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is a game object with a pickup system
        var pickupSystem = other.gameObject.GetComponent<PickupSystem>();
        if (pickupSystem != null)
        {
            // Collect the pickupable object
            var collected = pickupSystem.Pickup(pickupableData);
            if (collected)
            {
                PickupablesManager.Instance.DestroyPickupable(gameObject);
            }
        }
    }
}
