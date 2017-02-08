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

    private SurferPlayer player;

    public SurferState CurrentState { get { return currentState; } set { currentState = value; } }
    public PlayerLight CurrentLight { get { return playerLight; } set { playerLight = value; } }

    private int surferId = -1;
    public int SurferId { get { return surferId; } }

    // Use this for initialization
    void Start () {
        // TODO: Assert if we don't have a SurferPlayer
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
        if (player != null)
        {
            player = p;
            surferId = player.PlayerID;
        }
    }
}
