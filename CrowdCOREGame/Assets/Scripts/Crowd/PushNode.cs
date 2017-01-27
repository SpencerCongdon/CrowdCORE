using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushNode : MonoBehaviour {

    public float maxPush = 20000f;
    public float minPush = 20000f;
    public float pushRadius = 4f;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PushEveryone()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, pushRadius);
        foreach(Collider c in hitColliders)
        {
            if(c.attachedRigidbody != null)
            {
                Vector3 pushVector = (c.transform.position - this.transform.position).normalized * Random.Range(minPush, maxPush);
                c.attachedRigidbody.AddForce(pushVector);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pushRadius);
    }
}
