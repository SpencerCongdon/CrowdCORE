using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public Surfer followSurfer;
    public float offset = 1.0f;

    void Start ()
    {
    }

	void Update ()
    {
        if(followSurfer!= null)
        {
            this.transform.position = new Vector3(followSurfer.MainBody.position.x,
                                      followSurfer.MainBody.position.y + offset,
                                      followSurfer.MainBody.position.z);
        }
    }
}
