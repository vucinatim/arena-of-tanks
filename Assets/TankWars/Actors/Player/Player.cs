using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerID = -1; // Unique identifier for the player
    public PlayerData playerData;

    public Color color;

    [HideInInspector]
    public RenderTexture playerCameraRenderTexture;

    private ControlSystem controlSystem;

    void Awake()
    {
        // Check if the playerID not set
        if (playerID == -1)
        {
            Debug.LogError("PlayerData is null!");
            playerID = GetInstanceID();
            return;
        }

        // Set up the player's camera render texture
        Camera playerCamera = GetComponentInChildren<Camera>();
        playerCameraRenderTexture = new RenderTexture(playerData.playerRenderTexture);
        playerCamera.targetTexture = playerCameraRenderTexture;

        // Cache the control system
        controlSystem = GetComponent<ControlSystem>();
    }

    public void Initialize()
    {
        // Initialize all systems
        GetComponent<OverheadDisplaySystem>()
            .Initialize(this);
        GetComponent<ControlSystem>().Initialize(this, playerData.controlsData);
        GetComponent<AbilityQueueSystem>().Initialize(this, playerData.abilityQueueData);
        GetComponent<HealthSystem>().Initialize(this, playerData.healthData);
        GetComponent<LivesSystem>().Initialize(this, playerData.livesData);
        GetComponent<PhysicsMovementSystem>().Initialize(this, playerData.movementData);
        GetComponent<PickupSystem>().Initialize(this);
        GetComponent<StatusEffectsSystem>().Initialize(this, playerData.statusEffectsData);
        GetComponent<KnockbackSystem>().Initialize(this);

        // Subscribe to events
        EventManager.OnPlayerDeath += OnDeath;
    }

    public void SetInput(string input)
    {
        controlSystem.ButtonInput(input);
    }

    // Method to handle death
    protected void OnDeath(Player player, GameObject damageDealer)
    {
        if (player != this)
            return;
        var killer = damageDealer?.GetComponent<Player>();
        Debug.Log(
            $"(Player_{playerID}) Death - Killed by: {(killer != null ? killer.playerID : "Environment")}"
        );
        Instantiate(
            playerData.corpsePrefab,
            transform.position,
            transform.rotation,
            LevelManager.Instance.transform
        );
    }

    public void SetColor(Color color)
    {
        this.color = color;
        // Apply color to all parts of PlayerModel
        var playerTank = transform.Find("PlayerTank");
        foreach (Transform child in playerTank.transform)
        {
            if (!child.TryGetComponent<Renderer>(out var renderer))
                continue;
            renderer.material.color = color;
            if (child.childCount > 0)
            {
                foreach (Transform grandchild in child)
                {
                    grandchild.GetComponent<Renderer>().material.color = color;
                }
            }
        }
    }

    public void ResetPlayerForRespawn()
    {
        // Reset player for respawn
        var isControlSystemEnabled = GetComponent<ControlSystem>().enabled;
        GetComponent<ControlSystem>().enabled = false;
        GetComponent<AbilityQueueSystem>().ResetForRespawn();
        GetComponent<HealthSystem>().ResetForRespawn();
        GetComponent<StatusEffectsSystem>().ResetForRespawn();
        GetComponent<ControlSystem>().enabled = isControlSystemEnabled;
    }

    public void SetupForLobby()
    {
        gameObject.SetActive(true);
        // Disable the player's control system
        GetComponent<ControlSystem>().enabled = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<TankAudioSystem>().enabled = false;

        // Show the player's name plate
        GetComponent<OverheadDisplaySystem>()
            .ShowNamePlate(true);
        GetComponent<OverheadDisplaySystem>().ShowCurrentAbilityIcon(false);

        // Reset the players systems
        GetComponent<AbilityQueueSystem>()
            .TotalReset();
        GetComponent<HealthSystem>().TotalReset();
        GetComponent<StatusEffectsSystem>().TotalReset();
        GetComponent<PickupSystem>().TotalReset();
        GetComponent<LivesSystem>().TotalReset();
        GetComponent<AISystem>().enabled = false;
    }

    public void SetupForGameplay()
    {
        // Enable the player's control system
        GetComponent<ControlSystem>().enabled = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<TankAudioSystem>().enabled = true;

        // Hide the player's name plate
        GetComponent<OverheadDisplaySystem>()
            .ShowNamePlate(false);
        GetComponent<OverheadDisplaySystem>().ShowCurrentAbilityIcon(true);
        GetComponent<AISystem>().enabled = true;
    }
}
