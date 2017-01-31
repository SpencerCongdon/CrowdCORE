using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerControl : MonoBehaviour
{
    public enum PlayerState
    {
        MOVING,
        PUNCH,
        KICK
    }

	public Transform headBone;
	public float minRot = -45.0f;
	public float maxRot = 45.0f;
	public float rotSpeed = 2.0f;
	private float t = 0.0f;
	private Quaternion start;
	private Quaternion end;

	public List<Material> playerShirtMaterials;

	public List<SkinnedMeshRenderer> playerShirt;

	public float strikeChargeTime = 0.5f;
	public Transform MainBody;
	public float strikePower;

	public Transform RightArm;
	public Transform LeftArm;
	public Transform RightLeg;
	public Transform LeftLeg;

    private PlayerStats playerstats;
    private PlayerState currentState;


    void Awake()
	{
		start = headBone.localRotation * Quaternion.Euler(transform.right * minRot);
		end = headBone.localRotation * Quaternion.Euler(transform.right * maxRot);
        playerstats = GetComponent<PlayerStats>();


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
		// Head Bang
		t = Mathf.PingPong(Time.time * rotSpeed,1.0f);
		headBone.localRotation = Quaternion.Slerp (start, end, t);

        if(currentState == PlayerState.MOVING)
        {
            // Test Controls
            if (Input.GetButtonDown("AttackTop" + playerstats.PlayerID))
            {
                LimbStrike(LeftArm);
                LimbStrike(RightArm);
            }

            if (Input.GetButtonDown("AttackBot" + playerstats.PlayerID))
            {
                LimbStrike(LeftLeg);
                LimbStrike(RightLeg);
            }
        }
	}

	public void LimbStrike (Transform limb)
	{
		Vector3 bodyPosition = MainBody.position;
		Vector3 strikeDirection = (limb.position - bodyPosition);

        limb.transform.position += strikeDirection / 2;

        /*while (timer <= strikeChargeTime)
		{
			limb.gameObject.GetComponent<Rigidbody>().AddForce(strikeDirection * (-strikePower/4), ForceMode.Acceleration);
			timer += Time.deltaTime;
		}*/

		limb.gameObject.GetComponent<Rigidbody>().AddForce(-strikeDirection.normalized * strikePower, ForceMode.Impulse);
    }



}
