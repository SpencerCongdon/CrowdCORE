using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    void Start ()
    {
    }

	void Update ()
    {
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Player")
        {
            GameObject.Destroy(c.gameObject);
            Debug.Log(c.gameObject.name + " is TOTALLY FUCKING DEAD!!! \\m/ >_< \\m/");
        }
    }
}
