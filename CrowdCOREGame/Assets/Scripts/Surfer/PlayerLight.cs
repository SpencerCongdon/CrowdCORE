using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public GameObject followPlayer;
    public float offset = 1.0f;

    void Start ()
    {
    }

	void Update ()
    {
        if(followPlayer!= null)
        {
            this.transform.position = new Vector3(followPlayer.transform.position.x,
                                      followPlayer.transform.position.y + offset,
                                      followPlayer.transform.position.z);
        }
    }
}
