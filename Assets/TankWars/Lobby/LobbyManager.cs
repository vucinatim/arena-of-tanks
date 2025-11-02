using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField]
    private PlayerSpot[] playerSpots;
    private GameStateManager gameStateManager;
    private Dictionary<int, Player> playersInLobby;
    private Dictionary<int, PlayerSpot> spots;

    [SerializeField]
    private TextMeshProUGUI countdownText;

    private void Start()
    {
        gameStateManager = GameManager.Instance.GameStateManager;
        playersInLobby = new Dictionary<int, Player>();
        spots = new Dictionary<int, PlayerSpot>();

        InitializePlayerSpots();
    }

    private void InitializePlayerSpots()
    {
        playerSpots = GameObject
            .FindGameObjectsWithTag("LobbySpot")
            .Select(spot => spot.GetComponent<PlayerSpot>())
            .ToArray();
        for (int i = 0; i < playerSpots.Length; i++)
        {
            var spot = playerSpots[i];
            spot.Initialize(i, LevelManager.Instance.GetRandomSpawnPoint());
        }
        LevelManager.Instance.ResetSpawnPoints();
    }

    public void SetPlayerReady(int playerId, bool isReady)
    {
        if (!playersInLobby.ContainsKey(playerId))
        {
            Debug.LogError($"Player with ID {playerId} is not assigned to a spot!");
            return;
        }

        foreach (var spot in playerSpots)
        {
            if (spot.IsOccupiedBy(playersInLobby[playerId]))
            {
                spot.SetPlayerReady(playersInLobby[playerId], isReady);
                return;
            }
        }
    }

    public bool AllPlayersReady()
    {
        foreach (var spot in playerSpots)
        {
            if (spot.IsNotEmpty() && !spot.IsReady())
            {
                return false;
            }
        }

        return true;
    }

    public void AssignPlayerSpot(int playerId, Player player)
    {
        if (playersInLobby.ContainsKey(playerId))
        {
            Debug.LogWarning($"Player with ID {playerId} is already assigned to a spot!");
            return;
        }

        foreach (PlayerSpot spot in playerSpots)
        {
            if (spot.IsAvailable())
            {
                spot.OccupySpot(player);
                spots.Add(playerId, spot);
                playersInLobby.Add(playerId, player);
                return;
            }
        }

        Debug.LogError($"No available spot for player with ID {playerId}!");
    }

    public void RemovePlayer(int playerId)
    {
        if (!playersInLobby.ContainsKey(playerId))
        {
            Debug.LogError($"Player with ID {playerId} is not assigned to a spot!");
            return;
        }

        foreach (var spot in playerSpots)
        {
            if (spot.IsOccupiedBy(playersInLobby[playerId]))
            {
                spot.ClearSpot();
                playersInLobby.Remove(playerId);
                spots.Remove(playerId);

                return;
            }
        }
    }

    public void SetupPlayersForLobby()
    {
        foreach (KeyValuePair<int, Player> player in PlayerManager.Instance.players)
        {
            MovePlayerToLobby(player.Key);
            player.Value.SetupForLobby();
            SetPlayerReady(player.Key, false);
            PlayerManager.Instance.ResetPlayerReadyStates();
        }
    }

    public void SetupPlayersForGameplay()
    {
        foreach (KeyValuePair<int, Player> player in PlayerManager.Instance.players)
        {
            MovePlayerToLevel(player.Key);
            player.Value.SetupForGameplay();
        }
    }

    private void MovePlayerToLobby(int playerId)
    {
        if (!playersInLobby.ContainsKey(playerId))
        {
            Debug.LogError($"Player with ID {playerId} is not assigned to a spot!");
            return;
        }

        var player = playersInLobby[playerId];

        GameObject playerSpotPosition = spots[playerId].transform
            .Find("PlayerSpotPosition")
            .gameObject;
        player.transform.position = playerSpotPosition.transform.position;
        player.transform.rotation = playerSpotPosition.transform.rotation;
        player.transform.SetParent(playerSpotPosition.transform);
    }

    public void MovePlayerToLevel(int playerId)
    {
        if (!playersInLobby.ContainsKey(playerId))
        {
            Debug.LogError($"Player with ID {playerId} is not assigned to a spot!");
            return;
        }

        var player = playersInLobby[playerId];
        player.transform.position = spots[playerId].LevelPosition.position;
        player.transform.SetParent(PlayerManager.Instance.transform);
    }

    public void SetCountdown(int remainingSeconds)
    {
        if (remainingSeconds > 0)
        {
            countdownText.text = remainingSeconds.ToString();
            countdownText.gameObject.SetActive(true);
            // Add bounce animation code here
            // Scale up quickly and then scale back down to normal size to achieve a bounce effect
            LeanTween
                .scale(countdownText.gameObject, Vector3.one * 1.25f, 0.25f)
                .setEaseOutBack()
                .setOnComplete(() =>
                {
                    LeanTween
                        .scale(countdownText.gameObject, Vector3.one, 0.15f)
                        .setEaseInOutSine();
                });
        }
        else
        {
            countdownText.gameObject.SetActive(false);
        }
    }
}
