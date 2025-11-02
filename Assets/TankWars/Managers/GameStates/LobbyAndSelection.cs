using UnityEngine;
using System.Collections;

public class LobbyAndSelectionState : IGameState
{
    private LobbyManager lobbyManager;
    private PlayerManager playerManager;
    private GameManager gameManager;
    private Coroutine gameplayTransitionCoroutine;
    private CountdownController countdownController;

    public void Enter()
    {
        playerManager = PlayerManager.Instance;
        lobbyManager = LobbyManager.Instance;
        gameManager = GameManager.Instance;

        // Subscribe to events
        EventManager.OnPlayerAdded += OnPlayerAdded;
        EventManager.OnPlayerRemoved += OnPlayerRemoved;
        EventManager.OnPlayerReadyToggle += OnPlayerReadyToggle;

        countdownController = new CountdownController();
        countdownController.OnCountdownComplete += OnCountdownComplete;

        // Setup players for lobby
        lobbyManager.SetupPlayersForLobby();
    }

    public void Update()
    {
        // Handle lobby and selection state updates
        if (GameManager.useKeyboardInput)
        {
            HandleDebugInputs();
        }
        countdownController.Update();
    }

    public void Exit()
    {
        // Clean up
        if (gameplayTransitionCoroutine != null)
        {
            gameManager.StopCoroutine(gameplayTransitionCoroutine);
            gameplayTransitionCoroutine = null;
        }

        // Unsubscribe from events
        EventManager.OnPlayerAdded -= OnPlayerAdded;
        EventManager.OnPlayerRemoved -= OnPlayerRemoved;
        EventManager.OnPlayerReadyToggle -= OnPlayerReadyToggle;
        countdownController.OnCountdownComplete -= OnCountdownComplete;
    }

    private void OnCountdownComplete()
    {
        gameManager.GameStateManager.ChangeState(GameState.Gameplay);
    }

    private void HandleDebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Manually add a player
            var idx = playerManager.players.Count;
            playerManager.AddPlayer(idx);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            // Manually remove a player
            var idx = playerManager.players.Count - 1;
            playerManager.RemovePlayer(idx);
        }

        // Manually set player ready with numbers 1-5
        for (int i = 0; i < playerManager.players.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                playerManager.TogglePlayerReady(i);
            }
        }
    }

    private void OnPlayerAdded(Player newPlayer)
    {
        // Disable the player's control system
        newPlayer.transform.GetComponent<ControlSystem>().enabled = false;
        newPlayer.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        newPlayer.transform.GetComponent<TankAudioSystem>().enabled = false;

        countdownController.StopCountdown();

        // Assign the player to a spot in the lobby
        lobbyManager.AssignPlayerSpot(newPlayer.playerID, newPlayer);
    }

    private void OnPlayerRemoved(Player removedPlayer)
    {
        lobbyManager.RemovePlayer(removedPlayer.playerID);
    }

    private void OnPlayerReadyToggle(Player player, bool isReady)
    {
        lobbyManager.SetPlayerReady(player.playerID, isReady);

        // Start or stop the countdown based on players' readiness
        bool allPlayersReady = lobbyManager.AllPlayersReady();
        if (allPlayersReady)
        {
            AudioManager.Instance.PlayAnnouncer("Ready");
            countdownController.StartCountdown(3f); // Start a 3-second countdown
        }
        else
        {
            countdownController.StopCountdown();
            lobbyManager.SetCountdown(0); // Hide countdown
        }
    }
}
