using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum RarityType
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Pickupable", menuName = "Pickupables/Pickupable")]
public abstract class PickupableData : ScriptableObject
{
    public string pickupableName;
    public Sprite pickupableIcon;
    // public AudioClip pickupSound;
    public Color? pickupableColor;
    public GameObject pickupableVisual;
    public RarityType rarityType;

    // Function to call when this pickupable is collected
    public abstract bool OnCollected(GameObject collector);

    public GameObject GetPickupableVisual()
    {
        if (pickupableVisual == null)
        {
            return PickupablesManager.Instance.defaultPickupableVisual;
        }
        else
        {
            return pickupableVisual;
        }
    }
}