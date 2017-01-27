using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.5f, 0.2f, 0.6f);
        Gizmos.DrawSphere(transform.position, 1f);
    }
}
