using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for tracking base information about the surfer character
/// </summary>
[RequireComponent(typeof(SurferControl))]
public class Surfer : MonoBehaviour {

    public enum SurferState
    {
        ALIVE = 0,
        DEAD
    }

    [SerializeField]
    private SurferControl control;
    [SerializeField]
    private SurferState currentState = SurferState.ALIVE;
    [SerializeField]
    private PlayerLight playerLight;

    [SerializeField]
    [Tooltip("DEBUG: If false, this will auto assign a player id to the surfer")]
    private bool requirePlayer = true;

    private SurferPlayer player;

    public SurferState CurrentState { get { return currentState; } set { currentState = value; } }
    public PlayerLight CurrentLight { get { return playerLight; } set { playerLight = value; } }
    public Transform MainBody { get { return control.MainBody; } }

    [SerializeField]
    private Collider mainCollider;

    private int playerId = -1;
    public int SurferId { get { return playerId; } }

    // Use this for initialization
    void Start () {

        if(requirePlayer)
        {
            Debug.Assert(player != null, "Please make sure that the surfer is associated with a SurferPlayer");

            // If we start with a player, make sure to perform the set.
            // Generally a prefab of the surfer should not automatically include a player
            if (player != null) SetPlayer(player);
        }
        else
        {
            // Try anyway
            if (player != null)
            {
                SetPlayer(player);
            }
            else if(Rewired.ReInput.isReady && Rewired.ReInput.players.playerCount > 0)
            {
                // Just take a stab at it
                SetPlayer(new SurferPlayer(0));
            }
        }


    }

    
    public void OnHitDeadZone()
    {
        if (currentState != Surfer.SurferState.DEAD)
        {
            control.enabled = false;
            currentState = Surfer.SurferState.DEAD;
            playerLight.enabled = false;
            Light light = playerLight.GetComponent<Light>();
            light.enabled = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerDead(SurferId);
            }

            Debug.Log("Player " + SurferId + " is TOTALLY FUCKING DEAD!!! \\m/ >_< \\m/");
        }
    }

    // Update is called once per frame
    void Update () {

	}

    

    public void SetPlayer(SurferPlayer p)
    {
        if (player == null)
        {
            player = p;
            playerId = player.PlayerID;
            control.SetPlayerInput(playerId);
        }
    }
}
