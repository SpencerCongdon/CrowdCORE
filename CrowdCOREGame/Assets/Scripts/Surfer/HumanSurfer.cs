using UnityEngine;

/// <summary>
/// Used to represent the information of a Human-controlled surfer
/// </summary>
public class HumanSurfer : Surfer
{
    [SerializeField]
    private SurferInput _input;
    private SurferPlayer player;
    

    [SerializeField]
    [Tooltip("DEBUG: If false, this will auto assign a player id to the surfer")]
    private bool requirePlayer = true;

    // Use this for initialization
    void Start ()
    {
        if (requirePlayer)
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
            else if (Rewired.ReInput.isReady && Rewired.ReInput.players.playerCount > 0)
            {
                // Just take a stab at it
                SetPlayer(new SurferPlayer(0));
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Set the player controlling this Surfer
    /// </summary>
    /// <param name="newPlayer">The player to assign</param>
    public void SetPlayer(SurferPlayer newPlayer)
    {
        if (player == null)
        {
            player = newPlayer;
            surferId = player.PlayerID;
            _input.SetPlayerInput(surferId);
        }
    }
}
