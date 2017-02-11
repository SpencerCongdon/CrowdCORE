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
    private SurferPlayer player;

    public SurferState CurrentState { get { return currentState; } set { currentState = value; } }
    public PlayerLight CurrentLight { get { return playerLight; } set { playerLight = value; } }
    public Transform MainBody { get { return control.MainBody; } }

    private int playerId = -1;
    public int SurferId { get { return playerId; } }

    // Use this for initialization
    void Start () {
        Debug.Assert(player != null, "Please make sure that the surfer is associated with a SurferPlayer");

        // If we start with a player, make sure to perform the set.
        // Generally a prefab of the surfer should not automatically include a player
        if (player != null) SetPlayer(player);
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "DeadZone")
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
