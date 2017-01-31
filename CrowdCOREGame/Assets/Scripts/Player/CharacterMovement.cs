using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public enum MovementType
    {
        CONSTANT,
        IMPULSE
    }

    [SerializeField] private Transform topBody;
    [SerializeField] private Transform botBody;
    [SerializeField] private Rigidbody body;
    [SerializeField] private MovementType movementType;

    [SerializeField] private float cooldown;
    [SerializeField] private float topPower;
    [SerializeField] private float torquePower;

    private PlayerStats playerStats = null;

    private float cooldownTimerTop = 0;
    private float cooldownTimerBot = 0;

    void Start ()
    {
        playerStats = GetComponent<PlayerStats>();
    }
	
	void Update ()
    {
        if (cooldownTimerTop > 0)
        {
            cooldownTimerTop -= Time.deltaTime; ;
        }

        if (cooldownTimerBot > 0)
        {
            cooldownTimerBot -= Time.deltaTime; ;
        }

        switch (movementType)
        {
            case MovementType.CONSTANT:
                
                break;

            case MovementType.IMPULSE:

                Vector3 topDirection = new Vector3(-Input.GetAxis("HorizontalTop" + playerStats.PlayerID), 0, -Input.GetAxis("VerticalTop" + playerStats.PlayerID));
                Vector3 botDirection = new Vector3(Input.GetAxis("HorizontalBot" + playerStats.PlayerID), 0, -Input.GetAxis("VerticalBot" + playerStats.PlayerID));

                body.AddForce(topDirection.normalized * topPower);
                body.AddTorque(transform.forward * (botDirection.x* torquePower*Time.deltaTime));

                //.transform.Rotate(new Vector3(0, 0, botDirection.x * torquePower) );

                break;
        }
    }
}
