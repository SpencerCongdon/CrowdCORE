using System.Collections.Generic;
using UnityEngine;
using RewiredConsts;

[RequireComponent(typeof(PlayerStats))]
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

    // TODO: These should really be rigidbodies
    [SerializeField] private Transform mainBody;
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightLeg;
    [SerializeField] private Transform leftLeg;

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

    // Striking
	public float strikeChargeTime = 0.5f;
	public float strikePower;

    private PlayerStats playerstats;
    private PlayerState currentState;

    // For display
    [SerializeField] private float xMovement;
    [SerializeField] private float zMovement;


    void Awake()
	{
        playerstats = GetComponent<PlayerStats>();
        playerIn = Rewired.ReInput.players.GetPlayer(playerstats.PlayerID);

        start = headBone.localRotation * Quaternion.Euler(transform.right * minRot);
		end = headBone.localRotation * Quaternion.Euler(transform.right * maxRot);

        for (int i=0; i < playerShirt.Count; i++)
		{
			if(i != 0)
			{
				Material[] currentMats = playerShirt[i].materials;
				currentMats[1] = playerShirtMaterials[playerstats.PlayerID-1];
				playerShirt[i].materials = currentMats;

			}
			else
			{
				playerShirt[i].material = playerShirtMaterials[playerstats.PlayerID-1];
			}
		}

        currentState = PlayerState.MOVING;
    }

	void Update () 
	{
        UpdateHeadbanging();

        if(currentState == PlayerState.MOVING)
        {
            ProcessInput();
        }
	}

    private void UpdateHeadbanging()
    {
        // Head Bang
        t = Mathf.PingPong(Time.time * rotSpeed, 1.0f);
        headBone.localRotation = Quaternion.Slerp(start, end, t);
    }

    private void ProcessInput()
    {
        // Test Controls
        if(playerIn.GetButtonDown("Punch"))
        {
            LimbStrike(leftArm);
            LimbStrike(rightArm);
        }
            
        if (playerIn.GetButtonDown("Kick"))
        {
            LimbStrike(leftLeg);
            LimbStrike(rightLeg);
        }

        switch (movementType)
        {
            case MovementType.CONSTANT:
                {
                    xMovement = playerIn.GetAxis(Action.MoveHorizontal);
                    zMovement = playerIn.GetAxis(Action.MoveVertical);
                    Vector3 topDirection = new Vector3(xMovement, 0, zMovement);
                    mainBody.gameObject.GetComponent<Rigidbody>().AddForce(topDirection.normalized * topPower);
                    break;
                }

            case MovementType.IMPULSE:

                break;
        }
    }

	public void LimbStrike (Transform limb)
	{
		Vector3 bodyPosition = mainBody.position;
		Vector3 strikeDirection = (limb.position - bodyPosition);

        limb.transform.position += strikeDirection / 2;
		limb.gameObject.GetComponent<Rigidbody>().AddForce(-strikeDirection.normalized * strikePower, ForceMode.Impulse);
    }
}
