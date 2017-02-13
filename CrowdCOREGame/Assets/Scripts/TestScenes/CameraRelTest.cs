using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRelTest : MonoBehaviour {

    public GameObject InputPrefab;
    public GameObject SurferPrefab;

	// Use this for initialization
	void Start () {
        Instantiate(InputPrefab, new Vector3(0, 0, 0), new Quaternion());
        GameObject newSurfer = Instantiate(SurferPrefab, new Vector3(0, 5, 0), new Quaternion()) as GameObject;

        // Just assign first controller to it
        newSurfer.GetComponent<Surfer>().SetPlayer(new SurferPlayer(0));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
