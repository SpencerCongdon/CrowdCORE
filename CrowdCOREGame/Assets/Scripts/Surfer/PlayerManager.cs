using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerManager : Singleton<PlayerManager>
{

    private bool isSearching = false;

    // TODO: protect this?
    public List<SurferPlayer> Players;
    public int NumPlayers { get { return Players.Count; } }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(isSearching)
        {
            // This is the ReWired.Player count
            int playerCount = ReInput.players.playerCount;
            for(int i = 0; i < playerCount; i++)
            {
                if(ReInput.players.GetPlayer(i).GetButtonDown(RewiredConsts.Action.Accept))
                {
                    SurferPlayer newPlayer = new SurferPlayer(i);
                    Players.Add(newPlayer);
                }
            }
        }
    }

    public void StartSearchingForUsers(bool clearFirst = false)
    {
        // Don't start again if we are already searching
        if (isSearching) return;

        if (clearFirst) ClearPlayers();

        isSearching = true;
    }

    public void StopSearching()
    {
        isSearching = false;
    }

    private void ClearPlayers()
    {
        Players.Clear();
    }
}
