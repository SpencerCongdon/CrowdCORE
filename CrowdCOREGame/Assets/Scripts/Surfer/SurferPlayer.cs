using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to hold any information about the player that is controlling a surfer
/// </summary>
public class SurferPlayer
{
    [SerializeField] private int playerID;
    [SerializeField] private string playerName;

    public int PlayerID { get { return playerID; } set { playerID = value; } }
    public string PlayerName { get { return playerName; } }

    public SurferPlayer(int id)
    {
        playerID = id;
    }
}
