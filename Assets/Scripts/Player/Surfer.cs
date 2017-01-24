using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surfer : MonoBehaviour {

    public float MaxStamina = 100;
    public float CurrentStamina = 100;

    public float rightTrigger = 0.0f;
    public float leftTRigger = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        rightTrigger = Input.GetAxis("Right Trigger");
        leftTRigger = Input.GetAxis("Left Trigger");

	}


}
