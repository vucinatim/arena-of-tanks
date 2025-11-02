using System;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class ControllerEvents
{
    public const string ReadyPlayer = "ready-player";
    public const string PauseGame = "pause-game";
    public const string ResumeGame = "resume-game";
    public const string Control = "control";
}

public class GameManager : Singleton<GameManager>
{
    public static bool useKeyboardInput = true;
    private GameStateManager gameStateManager;
    public GameStateManager GameStateManager => gameStateManager;
    public GameState CurrentGameState => gameStateManager.currentState;

    public Dictionary<string, object> gameStateProperties =
        new(
            new KeyValuePair<string, object>[]
            {
                new("gameState", GameState.LobbyAndSelection.ToString()),
            }
        );

    private void Start()
    {
        gameStateManager = new GameStateManager();
        EventManager.OnGameStateChanged += OnGameStateChanged;

        // Register AirConsole event listeners
        AirConsole.instance.onReady += OnAirConsoleReady;
        AirConsole.instance.onMessage += OnAirConsoleMessage;
        AirConsole.instance.onConnect += OnAirConsoleConnect;
        AirConsole.instance.onDisconnect += OnAirConsoleDisconnect;
        AirConsole.instance.onCustomDeviceStateChange += OnAirConsoleCustomDeviceStateChange;
    }

    private void Update()
    {
        gameStateManager.Update();
    }

    private void OnDestroy()
    {
        // Unregister AirConsole event listeners
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onReady -= OnAirConsoleReady;
            AirConsole.instance.onMessage -= OnAirConsoleMessage;
            AirConsole.instance.onConnect -= OnAirConsoleConnect;
            AirConsole.instance.onDisconnect -= OnAirConsoleDisconnect;
            AirConsole.instance.onCustomDeviceStateChange -= OnAirConsoleCustomDeviceStateChange;
        }
    }

    private void OnGameStateChanged(GameState newState)
    {
        Debug.Log($"Game state changed to {newState}");
        if (AirConsole.instance.IsAirConsoleUnityPluginReady())
        {
            AirConsole.instance.SetCustomDeviceStateProperty("gameState", newState.ToString());
        }
    }

    private void OnAirConsoleReady(string code)
    {
        Debug.Log("AirConsole ready!");

        List<int> connectedDevices = AirConsole.instance.GetControllerDeviceIds();
        foreach (int deviceId in connectedDevices)
        {
            PlayerManager.Instance.AddPlayer(deviceId);
        }
    }

    private void OnAirConsoleMessage(int from, JToken message)
    {
        Debug.Log("Received message from player " + from + ": " + message);

        if (message["actionType"] == null)
            return;

        var actionType = message["actionType"].ToString();

        switch (actionType)
        {
            case ControllerEvents.ReadyPlayer:
                PlayerManager.Instance.TogglePlayerReady(from);
                AirConsole.instance.SetCustomDeviceStateProperty(
                    "playerReadyStates",
                    PlayerManager.Instance.playerReadyStates
                );
                break;
            case ControllerEvents.Control:
                PlayerManager.Instance.SetPlayerInput(from, message["action"].ToString());
                break;
        }
    }

    private void OnAirConsoleCustomDeviceStateChange(int from, JToken customDeviceState)
    {
        Debug.Log("Received custom device state from player " + from + ": " + customDeviceState);
    }

    private void OnAirConsoleConnect(int deviceId)
    {
        Debug.Log("Player " + deviceId + " connected.");
        PlayerManager.Instance.AddPlayer(deviceId);
    }

    private void OnAirConsoleDisconnect(int deviceId)
    {
        Debug.Log("Player " + deviceId + " disconnected.");
        PlayerManager.Instance.RemovePlayer(deviceId);
    }

    public void SlowDownTimeForDuration(float scale, float duration)
    {
        Time.timeScale = scale;

        // Start a coroutine to reset the time scale after the duration
        StartCoroutine(ResetTimeScaleAfterDuration(duration));
    }

    IEnumerator ResetTimeScaleAfterDuration(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
