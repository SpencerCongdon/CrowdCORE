﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using RewiredConsts;

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
                Player p = ReInput.players.GetPlayer(i);
                if (p.GetButtonDown(Action.JoinGame))
                {
                    SurferPlayer newPlayer = new SurferPlayer(i);
                    Players.Add(newPlayer);
                    p.controllers.maps.SetMapsEnabled(false, Category.Assignment);
                    p.controllers.maps.SetMapsEnabled(true, Category.Menu);
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
