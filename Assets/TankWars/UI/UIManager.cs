using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class UIController : MonoBehaviour
{
    public abstract void Show();
    public abstract void Hide();
}

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private UIController playersUI;

    [SerializeField]
    private UIController gameOverUI;

    private void Awake()
    {
        EventManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        EventManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.LobbyAndSelection:
                playersUI.Hide();
                gameOverUI.Hide();
                break;
            case GameState.Gameplay:
                playersUI.Show();
                gameOverUI.Hide();
                break;
            case GameState.GameOver:
                playersUI.Hide();
                gameOverUI.Show();
                break;
        }
    }
}
