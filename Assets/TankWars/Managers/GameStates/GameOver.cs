using UnityEngine;

public class GameOver : IGameState
{
    private CameraController cameraController;
    public void Enter()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        Transform cameraPositionLobby = GameObject.Find("CameraPositionLobby").transform;
        cameraController.isDynamic = false;
        cameraController.MoveToPosition(cameraPositionLobby.position, cameraPositionLobby.rotation, 2f);
        cameraController.ChangeFOV(40f, 2f);
        LobbyManager.Instance.SetupPlayersForLobby();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            GameManager.Instance.GameStateManager.ChangeState(GameState.LobbyAndSelection);
        }
    }

    public void Exit()
    {
        // Clean up the game over state
        LevelManager.Instance.Reset();
        PickupablesManager.Instance.ClearPickupables();
    }
}