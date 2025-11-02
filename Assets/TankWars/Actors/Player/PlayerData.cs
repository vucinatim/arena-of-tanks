using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Custom/Player Data", order = 1)]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public Texture2D playerIcon;
    public RenderTexture playerRenderTexture;
    public GameObject corpsePrefab;

    // --- Systems ---
    public AbilityQueueData abilityQueueData;
    public ControlsData controlsData;
    public HealthData healthData;

    public LivesData livesData;

    public PhysicsMovementData movementData;

    public StatusEffectsData statusEffectsData;

    public TankAnimationData tankAnimationData;
}
