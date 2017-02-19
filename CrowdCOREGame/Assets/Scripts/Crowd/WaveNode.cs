using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveNode : MonoBehaviour
{
    public float gizHeight = 3f;
    public float waveHalfWidth; // The width of the line from which the wave originates
    public float waveHalfExtent;    // How far out from the line the wave will influence

    public float waveSpeed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartWave()
    {


        //Collider[] hitColliders = Physics.OverlapBox(this.transform.position, new Vector3(waveHalfWidth, 0f, waveHalfExtent));
        //foreach (Collider c in hitColliders)
        //{
        //    if (c.attachedRigidbody != null)
        //    {
        //        Vector3 pushVector = (c.transform.position - this.transform.position).normalized * Random.Range(minPush, maxPush);
        //        c.attachedRigidbody.AddForce(pushVector);
        //    }
        //}

        float yPos = this.transform.position.y;
        Vector3 lineDir = new Vector3(1f, 0f, 0f);
        lineDir = this.transform.rotation * lineDir;
        int layerMask = 1 << 11;

        Collider[] hitColliders = Physics.OverlapBox(this.transform.position, new Vector3(waveHalfWidth, yPos, waveHalfExtent), new Quaternion(), layerMask);
        foreach (Collider c in hitColliders)
        {
            Vector3 collPos = c.transform.position;
            collPos.y = yPos;
            Vector3 nearPoint = NearestPointOnLine(this.transform.position, lineDir, collPos);
            Gizmos.DrawLine(collPos, nearPoint);
        }
    }

    

    //linePnt - point the line passes through
    //lineDir - unit vector in direction of line, either direction works
    //pnt - the point to find nearest on line for
    public Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();    //this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }

    void OnDrawGizmosSelected()
    {
        // Values used throughout the drawing
        Quaternion nodeRot = transform.rotation;
        float yPos = this.transform.position.y;

        // Draw the wave origin line
        Gizmos.color = new Color(1f, 0f, 0f);
        Vector3 from = nodeRot * new Vector3(-waveHalfWidth + transform.position.x, yPos, transform.position.z);
        Vector3 to = nodeRot * new Vector3(waveHalfWidth + transform.position.x, yPos, transform.position.z);
        Gizmos.DrawLine(from, to);

        // Draw the box for the wave area
        Gizmos.color = Color.blue;
        Quaternion rotation = nodeRot;
        Vector3 cor1 = rotation * new Vector3(waveHalfWidth, yPos, waveHalfExtent);
        Vector3 cor2 = rotation * new Vector3(waveHalfWidth, yPos, -waveHalfExtent);
        Vector3 cor3 = rotation * new Vector3(-waveHalfWidth, yPos, -waveHalfExtent);
        Vector3 cor4 = rotation * new Vector3(-waveHalfWidth, yPos, waveHalfExtent);

        Gizmos.DrawLine(cor1, cor2);
        Gizmos.DrawLine(cor2, cor3);
        Gizmos.DrawLine(cor3, cor4);
        Gizmos.DrawLine(cor4, cor1);

        // Draw the distances to crowd members influenced by the wave
        Gizmos.color = Color.yellow;
        Vector3 lineDir = nodeRot * new Vector3(1f, 0f, 0f);
        int layerMask = 1 << 11;
        Collider[] hitColliders = Physics.OverlapBox(this.transform.position, new Vector3(waveHalfWidth, yPos, waveHalfExtent), nodeRot, layerMask);
        foreach (Collider c in hitColliders)
        {
            Vector3 collPos = c.transform.position;
            collPos.y = yPos;
            Vector3 nearPoint = NearestPointOnLine(this.transform.position, lineDir, collPos);
            Gizmos.DrawLine(collPos, nearPoint);
        }
    }
}
