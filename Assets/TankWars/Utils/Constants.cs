using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static readonly Dictionary<RarityType, int> rarityWeights =
        new()
        {
            { RarityType.Common, 5 },
            { RarityType.Uncommon, 4 },
            { RarityType.Rare, 3 },
            { RarityType.Epic, 2 },
            { RarityType.Legendary, 1 }
        };

    public static readonly Dictionary<RarityType, Color> rarityColors =
        new()
        {
            [RarityType.Common] = new Color(0.6f, 0.6f, 0.6f, 1f), // Darker Gray
            [RarityType.Uncommon] = new Color(0.13f, 0.545f, 0.13f, 1f), // Forest Green
            [RarityType.Rare] = new Color(0.2f, 0.5f, 1f, 1f), // Lighter Blue
            [RarityType.Epic] = new Color(0.8f, 0f, 0.8f, 1f), // Vibrant Purple/Magenta
            [RarityType.Legendary] = new Color(1f, 0.6f, 0f, 1f) // More Orange
        };
}
