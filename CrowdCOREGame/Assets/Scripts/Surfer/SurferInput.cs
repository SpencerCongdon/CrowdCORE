using UnityEngine;
using RewiredConsts;

[RequireComponent(typeof(SurferControl))]
public class SurferInput : MonoBehaviour
{
    [SerializeField]
    private SurferControl control;
    private Rewired.Player playerIn;

    void Start()
	{
        if(control == null)
        {
            control = GetComponent<SurferControl>();
        }
    }

	void Update () 
	{
        if(control.CurrentState == SurferControl.PlayerState.Moving)
        {
            UpdateInput();
        }
	}

    public void SetPlayerInput(int playerId)
    {
        if(playerId >= 0)
        {
            playerIn = Rewired.ReInput.players.GetPlayer(playerId);
            playerIn.controllers.maps.SetMapsEnabled(true, Category.Surfer);
        }
    }


    private void UpdateInput()
    {
        // Drop out if no one is controlling us
        if (playerIn == null) return;

        control.PerformMovement(playerIn.GetAxis(ACTION.MoveHorizontal), playerIn.GetAxis(ACTION.MoveVertical));

        if (playerIn.GetButtonDown(ACTION.Punch))
        {
            control.PerformPunch();
        }
            
        if (playerIn.GetButtonDown(ACTION.Kick))
        {
            control.PerformKick();
        }
    }
}
