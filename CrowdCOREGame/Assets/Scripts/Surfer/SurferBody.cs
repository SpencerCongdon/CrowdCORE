using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurferBody : MonoBehaviour {

    public CCEvents.SurferHitDeadZone HitDeadZone;

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "DeadZone")
        {
            HitDeadZone.Invoke();
        }
    }
}


