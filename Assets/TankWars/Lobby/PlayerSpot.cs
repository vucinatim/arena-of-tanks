using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSpot : MonoBehaviour
{
    [SerializeField]
    public Color color;
    private Player occupant;

    [SerializeField]
    private Transform levelPosition;

    [SerializeField]
    private TextMeshProUGUI readyText;

    public Transform LevelPosition
    {
        get { return levelPosition; }
    }

    LTDescr rotateTween;

    private enum SpotState
    {
        Empty,
        Occupied,
        Ready
    }

    private SpotState state = SpotState.Empty;

    public void Initialize(int index, Transform levelPosition)
    {
        this.levelPosition = levelPosition;

        LeanTween.moveLocalY(gameObject, -5f, 0f);
        rotateTween = LeanTween.rotateAround(gameObject, Vector3.up, 360f, 8f).setLoopClamp();
        LeanTween.moveLocalY(gameObject, -1f, 1f).setEaseOutCubic().setDelay(1f + index * 0.4f);

        // Reset state
        ClearSpot();
    }

    public void ClearSpot()
    {
        // Trigger FX
        if (occupant != null)
        {
            FXManager.Instance.SpawnFX(
                "PlayerSpawn",
                occupant.transform.position,
                Quaternion.identity,
                gameObject.transform,
                3f
            );
            LeanTween.moveLocalY(gameObject, -1f, 1f).setEaseOutSine();
        }

        // Reset state
        state = SpotState.Empty;
        occupant = null;

        // Reset placeholder and player spot position active states
        transform.Find("PlayerTankPlaceholder").gameObject.SetActive(true);
        transform.Find("PlayerSpotPosition").gameObject.SetActive(false);
    }

    public void OccupySpot(Player player)
    {
        // Occupy this spot with a player
        state = SpotState.Occupied;
        occupant = player;

        // Animate the spot being occupied
        LeanTween.moveLocalY(gameObject, 0f, 1f).setEaseOutSine();
        LeanTween.scale(player.gameObject, Vector3.one * 0.8f, 0f);
        LeanTween.scale(player.gameObject, Vector3.one * 1.0f, 1f).setEaseOutElastic();

        // Update player position, rotation and parent
        GameObject playerSpotPosition = transform.Find("PlayerSpotPosition").gameObject;
        playerSpotPosition.SetActive(true);
        player.transform.position = playerSpotPosition.transform.position;
        player.transform.rotation = playerSpotPosition.transform.rotation;
        player.transform.SetParent(playerSpotPosition.transform);

        // Assign the player's color
        player.SetColor(color);

        // Trigger FX
        FXManager.Instance.SpawnFX(
            "PlayerSpawn",
            player.transform.position,
            Quaternion.identity,
            gameObject.transform,
            3f
        );

        // Update placeholder active state
        transform.Find("PlayerTankPlaceholder").gameObject.SetActive(false);
    }

    public void SetPlayerReady(Player player, bool isReady)
    {
        if (isReady)
        {
            PlayerReady(player);
        }
        else
        {
            PlayerUnready(player);
        }
    }

    private void PlayerReady(Player player)
    {
        if (state == SpotState.Occupied)
        {
            // Cancel any ongoing unready animations
            LeanTween.cancel(gameObject);

            // Player is ready
            state = SpotState.Ready;

            // Animate the spot showing ready state
            LeanTween.scale(player.gameObject, Vector3.one * 0.9f, 0.1f).setEaseOutSine();
            LeanTween
                .scale(player.gameObject, Vector3.one * 1f, 0.5f)
                .setEaseOutElastic()
                .setDelay(0.1f);

            LeanTween.rotateY(gameObject, 180f, 2f).setEaseOutElastic();

            // Show the ready text
            if (readyText != null)
            {
                readyText.gameObject.SetActive(true);
            }
        }
    }

    private void PlayerUnready(Player player)
    {
        if (state == SpotState.Ready)
        {
            // Cancel any ongoing ready animations
            LeanTween.cancel(gameObject);

            // Player is no longer ready
            state = SpotState.Occupied;

            // Reverse the "ready" animations
            LeanTween.scale(player.gameObject, Vector3.one * 1.0f, 0.5f).setEaseOutSine();
            LeanTween.rotateY(gameObject, 0f, 2f).setEaseInOutElastic();

            // Restart the rotation tween if needed
            rotateTween = LeanTween.rotateAround(gameObject, Vector3.up, 360f, 8f).setLoopClamp();

            // Hide the ready text
            if (readyText != null)
            {
                readyText.gameObject.SetActive(false);
            }
        }
    }

    public bool IsAvailable()
    {
        return state == SpotState.Empty;
    }

    public bool IsOccupiedBy(Player player)
    {
        return (state == SpotState.Occupied || state == SpotState.Ready) && occupant == player;
    }

    public bool IsReady()
    {
        return state == SpotState.Ready;
    }

    public bool IsNotEmpty()
    {
        return state != SpotState.Empty;
    }
}
