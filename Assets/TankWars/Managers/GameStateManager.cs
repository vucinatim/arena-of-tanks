using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    LobbyAndSelection,
    Gameplay,
    GameOver,
}

public interface IGameState
{
    void Enter();
    void Update();
    void Exit();
}

public class GameStateManager
{
    private Dictionary<GameState, IGameState> gameStates;

    private IGameState currentStateValue;
    public GameState currentState { get; private set; }

    private void SetCurrentState(GameState newState)
    {
        Debug.Log($"Changing state from {currentState} to {newState}");
        currentState = newState;
        currentStateValue = gameStates[currentState];
    }

    public GameStateManager()
    {
        Debug.Log("Initializing GameStateManager");
        gameStates = new Dictionary<GameState, IGameState>
        {
            { GameState.LobbyAndSelection, new LobbyAndSelectionState() },
            { GameState.Gameplay, new Gameplay() },
            { GameState.GameOver, new GameOver() },
        };
        SetCurrentState(GameState.LobbyAndSelection);
        EventManager.TriggerGameStateChanged(GameState.LobbyAndSelection);
        currentStateValue.Enter();
    }

    public void Update()
    {
        currentStateValue.Update();
    }

    public void ChangeState(GameState newState)
    {
        currentStateValue.Exit();
        SetCurrentState(newState);
        currentStateValue.Enter();
        EventManager.TriggerGameStateChanged(newState);
    }
}
