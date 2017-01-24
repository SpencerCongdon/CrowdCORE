using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBody : MonoBehaviour
{
    [SerializeField] private PlayerStats playerstats;
    [SerializeField] private PlayerControl playerControl;
    [SerializeField] private CharacterMovement characterMovement;

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "DeadZone")
        {
            if (playerstats.CurrentState != PlayerStats.PlayerState.DEAD)
            {
                playerControl.enabled = false;
                characterMovement.enabled = false;
                playerstats.CurrentState = PlayerStats.PlayerState.DEAD;
                playerstats.CurrentLight.enabled = false;
                Light light = playerstats.CurrentLight.GetComponent<Light>();
                light.enabled = false;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnPlayerDead(playerstats.PlayerID);
                }                

                Debug.Log("Player " + playerstats.PlayerID + " is TOTALLY FUCKING DEAD!!! \\m/ >_< \\m/");
            }            
        }
    }

}
