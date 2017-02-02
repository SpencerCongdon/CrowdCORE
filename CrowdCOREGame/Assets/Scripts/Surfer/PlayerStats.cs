using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public enum PlayerState
    {
        ALIVE = 0,
        DEAD
    }

    [SerializeField] private int playerID;
    [SerializeField] private string playerName;

    private PlayerState currentState = PlayerState.ALIVE;
    private PlayerLight playerLight;

    public int PlayerID { get { return playerID; } set { playerID = value; } }
    public string PlayerName { get { return playerName; } }
    public PlayerState CurrentState { get { return currentState; } set { currentState = value; } }
    public PlayerLight CurrentLight { get { return playerLight; } set { playerLight = value; } }
}
