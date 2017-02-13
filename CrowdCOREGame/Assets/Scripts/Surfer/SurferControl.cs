using System.Collections.Generic;
using UnityEngine;
using RewiredConsts;

[RequireComponent(typeof(Surfer))]
public class SurferControl : MonoBehaviour
{
    public enum PlayerState
    {
        MOVING,
        PUNCH,
        KICK
    }

    public enum MovementType
    {
        CONSTANT,
        IMPULSE
    }

    // Necessary for operation
    [SerializeField]
    private Surfer surfer;

    // TODO: These should really be rigidbodies
    [SerializeField] private Transform mainBody;
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightLeg;
    [SerializeField] private Transform leftLeg;

    public Transform MainBody { get { return mainBody; } }

    // Movement
    [SerializeField]
    private MovementType movementType;
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private float topPower;
    [SerializeField]
    private float torquePower;

    // Headbanging
    public Transform headBone;
	public float minRot = -45.0f;
	public float maxRot = 45.0f;
	public float rotSpeed = 2.0f;
	private float t = 0.0f;
	private Quaternion start;
	private Quaternion end;

    private Rewired.Player playerIn;

	public List<Material> playerShirtMaterials;
	public List<SkinnedMeshRenderer> playerShirt;

    private Camera surfCamera;
    [SerializeField]
    private bool isCameraRelative = true;

    // Striking
	public float strikeChargeTime = 0.5f;
	public float strikePower;

    private PlayerState currentState;

    [SerializeField] private float xMoveInput;
    [SerializeField] private float zMoveInput;
    [SerializeField] private Vector3 lastMovement;


    void Start()
	{
        start = headBone.localRotation * Quaternion.Euler(transform.right * minRot);
        end = headBone.localRotation * Quaternion.Euler(transform.right * maxRot);

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

        currentState = PlayerState.MOVING;
    }

	void Update () 
	{
        // TODO: Why is headbanging part of control?
        UpdateHeadbanging();

        if(currentState == PlayerState.MOVING)
        {
            ProcessInput();
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

    public void LimbStrike(Transform limb)
    {
        Vector3 bodyPosition = mainBody.position;
        Vector3 strikeDirection = (limb.position - bodyPosition);

        limb.transform.position += strikeDirection / 2;
        limb.gameObject.GetComponent<Rigidbody>().AddForce(-strikeDirection.normalized * strikePower, ForceMode.Impulse);
    }

    private void UpdateHeadbanging()
    {
        // Head Bang
        t = Mathf.PingPong(Time.time * rotSpeed, 1.0f);
        headBone.localRotation = Quaternion.Slerp(start, end, t);
    }

    private void ProcessInput()
    {
        // Drop out if no one is controlling us
        if (playerIn == null) return;
         
        // Test Controls
        if(playerIn.GetButtonDown(ACTION.Punch))
        {
            LimbStrike(leftArm);
            LimbStrike(rightArm);
        }
            
        if (playerIn.GetButtonDown(ACTION.Kick))
        {
            LimbStrike(leftLeg);
            LimbStrike(rightLeg);
        }

        switch (movementType)
        {
            case MovementType.CONSTANT:
                {
                    // Update these now
                    xMoveInput = playerIn.GetAxis(ACTION.MoveHorizontal);
                    zMoveInput = playerIn.GetAxis(ACTION.MoveVertical);

                    Vector3 movement = Vector3.zero;
                    if(isCameraRelative && surfCamera != null)
                    {
                        movement = CalculateRelativeMovement();
                    }
                    else
                    {
                        movement = CalculateNormalMovement();
                    }

                    lastMovement = movement;

                    // Move if necessary
                    if(movement != Vector3.zero)
                    {
                        mainBody.gameObject.GetComponent<Rigidbody>().AddForce(movement.normalized * topPower);
                    }
                    
                    break;
                }

            case MovementType.IMPULSE:

                break;
        }
    }

    private Vector3 CalculateNormalMovement()
    {
        return new Vector3(xMoveInput, 0, zMoveInput);
    }

    private Vector3 CalculateRelativeMovement()
    {
        Vector3 move = (surfCamera.transform.right * xMoveInput) + (surfCamera.transform.forward * zMoveInput);
        move.y = 0;
        return move;
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

        if(isCameraRelative)
        {
            // Draw camera forward
            Gizmos.color = Color.red;
            toPoint = fromPoint + (surfCamera.transform.forward * SCALE);
            toPoint.y = Y_POS;
            Gizmos.DrawLine(fromPoint, toPoint);
        }
    }
}
