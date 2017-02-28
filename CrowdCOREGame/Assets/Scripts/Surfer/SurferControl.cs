using System.Collections.Generic;
using UnityEngine;
using RewiredConsts;

[RequireComponent(typeof(Surfer))]
public class SurferControl : MonoBehaviour
{
    public enum PlayerState
    {
        Moving,
        Punch,
        Kick
    }

    public enum RollStatus
    {
        FacingUp,
        FacingDown
    }

    // Necessary for operation
    [SerializeField]
    private Surfer surfer;

    // TODO: These should really be rigidbodies
    [SerializeField] private Rigidbody mainBody;
    [SerializeField] private Rigidbody rightHand;
    [SerializeField] private Rigidbody leftHand;
    [SerializeField] private Rigidbody rightFoot;
    [SerializeField] private Rigidbody leftFoot;

    public Rigidbody MainBody { get { return mainBody; } }

    // Movement
    [SerializeField] private float movementPower;
    [SerializeField] private float rollPower;

    private Rewired.Player playerIn;

	public List<Material> playerShirtMaterials;
	public List<SkinnedMeshRenderer> playerShirt;

    private Camera surfCamera;
    [SerializeField]
    private bool isCameraRelative = true;

    // Striking
	public float strikeChargeTime = 0.5f;
	public float strikePower;

    [SerializeField] private PlayerState currentState;
    [SerializeField] private RollStatus currentRoll;

    // For inspector
    [SerializeField] private Vector3 lastMovement;

    void Start()
	{
        // TODO: Why is this part of control
        for (int i = 0; i < playerShirt.Count; i++)
        {
            if (i != 0)
            {
                Material[] currentMats = playerShirt[i].materials;
                currentMats[1] = playerShirtMaterials[0]; // Change material
                playerShirt[i].materials = currentMats;
            }
            else
            {
                playerShirt[i].material = playerShirtMaterials[0]; // Change material
            }
        }

        surfCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        currentState = PlayerState.Moving;
    }

	void Update () 
	{
        // Everything we need to update all the time
        UpdateSurferRoll();

        if(currentState == PlayerState.Moving)
        {
            UpdateInput();
        }
	}

    void UpdateSurferRoll()
    {
        float forwardY = mainBody.transform.forward.y;
        if(currentRoll == RollStatus.FacingDown && forwardY >= 0)
        {
            currentRoll = RollStatus.FacingUp;
        }
        else if(currentRoll == RollStatus.FacingUp && forwardY < 0)
        {
            currentRoll = RollStatus.FacingDown;
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

    public void LimbStrike(Rigidbody limb)
    {
        Vector3 bodyPosition = mainBody.position;
        Vector3 strikeDirection = (limb.position - bodyPosition);

        limb.transform.position += strikeDirection / 2;
        limb.AddForce(-strikeDirection.normalized * strikePower, ForceMode.Impulse);
    }

    private void UpdateInput()
    {
        // Drop out if no one is controlling us
        if (playerIn == null) return;

        PerformMovement(playerIn.GetAxis(ACTION.MoveHorizontal), playerIn.GetAxis(ACTION.MoveVertical));

        if (playerIn.GetButtonDown(ACTION.Punch))
        {
            PerformPunch();
        }
            
        if (playerIn.GetButtonDown(ACTION.Kick))
        {
            PerformKick();
        }
    }

    private void PerformMovement(float xMovement, float yMovement)
    {
        Vector3 movement = Vector3.zero;
        if (isCameraRelative && surfCamera != null)
        {
            movement = (surfCamera.transform.right * xMovement) + (surfCamera.transform.forward * yMovement);
            movement.y = 0;
        }
        else
        {
            movement = new Vector3(xMovement, 0, yMovement);
        }

        lastMovement = movement;

        // Move if necessary
        if (movement != Vector3.zero)
        {
            mainBody.AddForce(movement.normalized * movementPower);
        }
    }

    private void PerformRoll()
    {
        mainBody.AddRelativeTorque(new Vector3(0, rollPower, 0));
    }

    private void PerformPunch()
    {
        LimbStrike(leftHand);
        LimbStrike(rightHand);
    }

    private void PerformKick()
    {
        LimbStrike(leftFoot);
        LimbStrike(rightFoot);
    }


    void OnDrawGizmos()
    {
        const float Y_POS = 0.1f;
        const float SCALE = 10;

        // Draw movement result
        Gizmos.color = Color.blue;
        Vector3 fromPoint = mainBody.position;
        fromPoint.y = Y_POS;
        Vector3 toPoint = new Vector3(fromPoint.x + (lastMovement.x * SCALE), Y_POS, fromPoint.z + (lastMovement.z * SCALE));
        Gizmos.DrawLine(fromPoint, toPoint);

        if(isCameraRelative && surfCamera != null)
        {
            // Draw camera forward
            Gizmos.color = Color.red;
            toPoint = fromPoint + (surfCamera.transform.forward * SCALE);
            toPoint.y = Y_POS;
            Gizmos.DrawLine(fromPoint, toPoint);
        }
    }
}
