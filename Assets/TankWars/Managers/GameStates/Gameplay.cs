using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gameplay : IGameState
{
    private CameraController cameraController;
    private CountdownController countdownController;
    private AudioSource gameMusic;

    private PlayerManager playerManager;
    private GameManager gameManager;
    private LobbyManager lobbyManager;
    private StatsManager statsManager;
    private LevelManager levelManager;
    private List<Coroutine> coroutines;

    public List<Player> eliminatedPlayers;
    public int controlledPlayerID;

    public void Enter()
    {
        eliminatedPlayers = new List<Player>();
        coroutines = new List<Coroutine>();

        // Cache manager instances
        playerManager = PlayerManager.Instance;
        gameManager = GameManager.Instance;
        lobbyManager = LobbyManager.Instance;
        statsManager = StatsManager.Instance;
        levelManager = LevelManager.Instance;

        // Subscribe to events
        EventManager.OnPlayerEliminated += OnPlayerEliminated;
        EventManager.OnPlayerRespawnTimerStart += OnPlayerRespawn;

        countdownController = new CountdownController();
        countdownController.StartCountdown(3);
        // Subscribe to TimerManager events
        countdownController.OnCountdownComplete += OnBattleBegin;

        // Initialize the gameplay state
        cameraController = Camera.main.GetComponent<CameraController>();

        // Get main camera and move it to the gameplay position
        Transform cameraPositionGameplay = GameObject.Find("CameraPositionGameplay").transform;
        cameraController.ChangeFOV(60f, 4f);
        cameraController.isDynamic = true;
        // cameraController.MoveToPosition(cameraPositionGameplay.position, cameraPositionGameplay.rotation, 4f);

        // Setup players for the gameplay
        lobbyManager.SetupPlayersForGameplay();

        // Set the controlled player ID with the first player in the players dictionary
        controlledPlayerID = playerManager.players.Keys.ElementAt(0);

        // Start spawning powerups
        PickupablesManager.Instance.StartSpawningPickupables();

        // Start the game music
        // gameMusic.Play();
    }

    public void Update()
    {
        // Handle countdown updates
        countdownController.Update();
        // Handle gameplay state updates
        // You can add any game-specific logic that needs to be executed during the gameplay state
        if (!GameManager.useKeyboardInput) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            playerManager.players[controlledPlayerID].GetComponent<HealthSystem>().ApplyDamage(null, 1000f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerManager.SetPlayerInput(controlledPlayerID, "ability");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            playerManager.SetPlayerInput(controlledPlayerID, "up");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            playerManager.SetPlayerInput(controlledPlayerID, "down");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            playerManager.SetPlayerInput(controlledPlayerID, "left");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            playerManager.SetPlayerInput(controlledPlayerID, "right");
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            playerManager.SetPlayerInput(controlledPlayerID, "up-up");
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            playerManager.SetPlayerInput(controlledPlayerID, "down-up");
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            playerManager.SetPlayerInput(controlledPlayerID, "left-up");
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            playerManager.SetPlayerInput(controlledPlayerID, "right-up");
        }
    }

    public void Exit()
    {
        // Stop all coroutines
        foreach (Coroutine coroutine in coroutines)
        {
            gameManager.StopCoroutine(coroutine);
        }

        gameManager.SlowDownTimeForDuration(0.3f, 2f);
        countdownController.StopCountdown();


        PickupablesManager.Instance.StopSpawningPickupables();

        // Unsubscribe from events
        EventManager.OnPlayerRespawnTimerStart -= OnPlayerRespawn;
        EventManager.OnPlayerEliminated -= OnPlayerEliminated;
    }

    private void OnBattleBegin()
    {
        // Enable players for the gameplay
        foreach (KeyValuePair<int, Player> player in PlayerManager.Instance.players)
        {
            player.Value.GetComponent<ControlSystem>().enabled = true;
            player.Value.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        AudioManager.Instance.PlayAnnouncer("BattleBegin");
    }

    private void OnPlayerEliminated(Player eliminatedPlayer)
    {
        eliminatedPlayer.gameObject.SetActive(false);
        eliminatedPlayers.Add(eliminatedPlayer);
        int rank = playerManager.players.Count - eliminatedPlayers.Count + 1;
        statsManager.SetPlayerRank(eliminatedPlayer, rank);

        if (eliminatedPlayers.Count == playerManager.players.Count - 1)
        {
            gameManager.GameStateManager.ChangeState(GameState.GameOver);
        }
    }

    private void OnPlayerRespawn(Player player, float delay)
    {
        player.gameObject.SetActive(false);
        coroutines.Add(gameManager.StartCoroutine(RespawnPlayerCoroutine(player, delay)));
    }

    private IEnumerator RespawnPlayerCoroutine(Player player, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Find the furthest spawn point from other players
        Transform spawnPoint = levelManager.GetFurthestSpawnPoint(PlayerManager.Instance.GetPlayerTransforms());

        // Check if a spawn point was found
        if (spawnPoint == null)
        {
            Debug.LogError("No available spawn point found!");
            yield break;
        }

        // Set the player position to the spawn point
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;

        // Enable the player object
        player.ResetPlayerForRespawn();
        player.gameObject.SetActive(true);

        EventManager.TriggerPlayerRespawned(player);
    }
}