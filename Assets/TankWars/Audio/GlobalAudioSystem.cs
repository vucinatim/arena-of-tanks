using UnityEngine;

class GlobalAudioSystem : MonoBehaviour
{

    void Awake()
    {
        // Subscribe to events
        EventManager.OnPlayerAdded += OnPlayerAdded;
        EventManager.OnPlayerRemoved += OnPlayerRemoved;
        EventManager.OnPlayerReadyToggle += OnPlayerReadyToggle;
        EventManager.OnPlayerRespawned += OnPlayerRespawned;
        EventManager.OnPlayerEliminated += OnPlayerEliminated;
        EventManager.OnGameStateChanged += OnGameStateChanged;
        EventManager.OnCountdownComplete += OnCountdownComplete;
        EventManager.OnCountdownUpdated += OnCountdownUpdated;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        EventManager.OnPlayerAdded -= OnPlayerAdded;
        EventManager.OnPlayerRemoved -= OnPlayerRemoved;
        EventManager.OnPlayerReadyToggle -= OnPlayerReadyToggle;
        EventManager.OnPlayerRespawned -= OnPlayerRespawned;
        EventManager.OnPlayerEliminated -= OnPlayerEliminated;
        EventManager.OnGameStateChanged -= OnGameStateChanged;
        EventManager.OnCountdownComplete -= OnCountdownComplete;
        EventManager.OnCountdownUpdated -= OnCountdownUpdated;
    }

    private void OnPlayerAdded(Player player)
    {
        AudioManager.Instance.PlaySFX("PlayerJoined");
    }

    private void OnPlayerRemoved(Player player)
    {
        AudioManager.Instance.PlaySFX("PlayerLeft");
    }

    private void OnPlayerReadyToggle(Player player, bool isReady)
    {
        AudioManager.Instance.PlaySFX("GeneralButton");
    }

    private void OnPlayerRespawned(Player player)
    {
        AudioManager.Instance.PlaySFX("PlayerRespawn");
    }

    private void OnPlayerEliminated(Player player)
    {
        AudioManager.Instance.PlaySFX("PlayerEliminated");
        AudioManager.Instance.PlayAnnouncer("Eliminated");
    }

    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.LobbyAndSelection:
                AudioManager.Instance.PlayMusic("Lobby");
                break;
            case GameState.Gameplay:
                AudioManager.Instance.PlaySFX("GameStart");
                AudioManager.Instance.PlayMusic("Gameplay");
                break;
            case GameState.GameOver:
                AudioManager.Instance.PlayMusic("GameOver");
                break;
        }
    }

    private void OnCountdownComplete()
    {
        AudioManager.Instance.PlaySFX("CountdownComplete");
    }

    private void OnCountdownUpdated(int time)
    {
        AudioManager.Instance.PlaySFX("CountdownTick");
    }
}