using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public GameObject playerPrefab;
    public Dictionary<int, Player> players = new();
    public Dictionary<int, bool> playerReadyStates = new();

    protected override void Awake()
    {
        base.Awake();
    }

    public List<Transform> GetPlayerTransforms()
    {
        List<Transform> playerTransforms = new List<Transform>();

        foreach (KeyValuePair<int, Player> player in players)
        {
            if (player.Value == null) continue;
            playerTransforms.Add(player.Value.transform);
        }

        return playerTransforms;
    }

    public void AddPlayer(int deviceId)
    {
        if (players.ContainsKey(deviceId))
        {
            Debug.LogWarning("Player with deviceId " + deviceId + " already exists!");
            return;
        }

        GameObject newPlayerObject = Instantiate(playerPrefab);
        Player newPlayer = newPlayerObject.GetComponent<Player>();
        newPlayer.playerID = deviceId;
        players.Add(deviceId, newPlayer);
        playerReadyStates.Add(deviceId, false);
        EventManager.TriggerPlayerAdded(newPlayer);
        newPlayer.Initialize();
    }

    public void RemovePlayer(int deviceId)
    {
        if (!players.ContainsKey(deviceId))
        {
            Debug.LogWarning("Player with deviceId " + deviceId + " does not exist!");
            return;
        }

        Player playerToRemove = players[deviceId];
        EventManager.TriggerPlayerRemoved(playerToRemove);
        players.Remove(deviceId);
        playerReadyStates.Remove(deviceId);
        Destroy(playerToRemove.gameObject);
    }

    public void TogglePlayerReady(int deviceId)
    {
        if (!players.ContainsKey(deviceId))
        {
            Debug.LogWarning("Player with deviceId " + deviceId + " does not exist!");
            return;
        }

        playerReadyStates[deviceId] = !playerReadyStates[deviceId];
        EventManager.TriggerPlayerReadyToggle(players[deviceId], playerReadyStates[deviceId]);
    }

    public void ResetPlayerReadyStates()
    {
        var newPlayerReadyStates = new Dictionary<int, bool>();
        foreach (KeyValuePair<int, Player> player in players)
        {
            newPlayerReadyStates.Add(player.Key, false);
        }

        playerReadyStates = newPlayerReadyStates;
    }

    public void SetPlayerInput(int deviceId, string input)
    {
        if (!players.ContainsKey(deviceId))
        {
            Debug.LogWarning("Player with deviceId " + deviceId + " does not exist!");
            return;
        }

        Player player = players[deviceId];
        player.SetInput(input);
    }
}

