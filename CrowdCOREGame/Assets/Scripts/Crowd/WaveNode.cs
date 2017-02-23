using UnityEngine;

public class WaveNode : MonoBehaviour
{
    private const float OVERLAP_HEIGHT = 2f; // Height we'll be counting jumpers

    public float waveHalfWidth;     // The width of the line from which the wave originates
    public float waveHalfExtent;    // How far out from the line the wave will influence
    public float minHeight;         // Height we want people to jump
    public float maxHeight;         // Factor by which jumps will vary
    public float waveDuration;      // Time that the wave is active
    public float waveSpeed;         // Modifies how fast the jump propegates through the crowd

    /// <summary>
    /// Initiate a wave at the current location
    /// </summary>
    public void StartWave()
    {
        float yPos = this.transform.position.y;
        Vector3 lineDir = Vector3.right;
        lineDir = this.transform.rotation * lineDir;
        int layerMask = 1 << 11; // Only hit Crowd Members

        Collider[] hitColliders = Physics.OverlapBox(this.transform.position, new Vector3(waveHalfWidth, OVERLAP_HEIGHT, waveHalfExtent), this.transform.rotation, layerMask);
        foreach (Collider c in hitColliders)
        {
            CrowdMember jumper = c.GetComponent<CrowdMember>();
            Debug.Assert(jumper != null, "GameObject on CrowdMember layer doesn't have a CrowdMember script");

            if(jumper != null)
            {
                Vector3 collPos = c.transform.position;
                collPos.y = yPos;
                Vector3 nearPoint = NearestPointOnLine(this.transform.position, lineDir, collPos);
                float distanceToOrigin = (nearPoint - collPos).magnitude;
                float delay = waveSpeed * distanceToOrigin;
                
                // TODO: The crowd member should be calculating these, only pass the jump height
                //float velocity = VelocityForJump(minHeight);
                //float interval = TimeForJump(velocity);
                jumper.JoinWave(delay, waveDuration, minHeight, maxHeight);
            }
        }
    }

    /// <summary>
    /// A function to find the point on a line closest to an external point
    /// </summary>
    /// <param name="linePnt">Point the line passes through</param>
    /// <param name="lineDir">Unit vector in direction of line</param>
    /// <param name="pnt">The external point</param>
    /// <returns></returns>
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
        Quaternion nodeRot = this.transform.rotation;
        Vector3 nodePos = this.transform.position;
        float xPos = nodePos.x;
        float yPos = nodePos.y;
        float zPos = nodePos.z;

        // Draw the wave origin line
        Gizmos.color = new Color(1f, 0f, 0f);
        Vector3 from = new Vector3(-waveHalfWidth + xPos, yPos, zPos);
        Vector3 to = new Vector3(waveHalfWidth + xPos, yPos, zPos);

        from = RotatePointAroundPivot(from, nodePos, nodeRot);
        to = RotatePointAroundPivot(to, nodePos, nodeRot);
        Gizmos.DrawLine(from, to);

        // Draw the box for the wave area
        Gizmos.color = Color.blue;

        // The corner locations
        Vector3 cor1 = new Vector3(waveHalfWidth + xPos,  yPos, waveHalfExtent + zPos);
        Vector3 cor2 = new Vector3(waveHalfWidth + xPos,  yPos, -waveHalfExtent + zPos);
        Vector3 cor3 = new Vector3(-waveHalfWidth + xPos, yPos, -waveHalfExtent + zPos);
        Vector3 cor4 = new Vector3(-waveHalfWidth + xPos, yPos, waveHalfExtent + zPos);

        // Rotate them with the node
        cor1 = RotatePointAroundPivot(cor1, nodePos, nodeRot);
        cor2 = RotatePointAroundPivot(cor2, nodePos, nodeRot);
        cor3 = RotatePointAroundPivot(cor3, nodePos, nodeRot);
        cor4 = RotatePointAroundPivot(cor4, nodePos, nodeRot);

        Gizmos.DrawLine(cor1, cor2);
        Gizmos.DrawLine(cor2, cor3);
        Gizmos.DrawLine(cor3, cor4);
        Gizmos.DrawLine(cor4, cor1);

        // Draw the distances to crowd members influenced by the wave
        Gizmos.color = Color.yellow;
        Vector3 lineDir = nodeRot * new Vector3(1f, 0f, 0f);
        int layerMask = 1 << 11;
        Collider[] hitColliders = Physics.OverlapBox(nodePos, new Vector3(waveHalfWidth, OVERLAP_HEIGHT, waveHalfExtent), nodeRot, layerMask);
        foreach (Collider c in hitColliders)
        {
            Vector3 collPos = c.transform.position;
            collPos.y = yPos;
            Vector3 nearPoint = NearestPointOnLine(nodePos, lineDir, collPos);
            Gizmos.DrawLine(collPos, nearPoint);
        }
    }

    /// <summary>
    /// For rotating gizmo points
    /// </summary>
    /// <param name="point">Point to rotate</param>
    /// <param name="pivot">Point to rotate around</param>
    /// <param name="rotation">The rotation to perform</param>
    /// <returns>The rotated point</returns>
    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        return rotation * (point - pivot) + pivot;
    }

    private float VelocityForJump(float desiredHeight)
    {
        float g = Physics.gravity.magnitude;
        float initialV = Mathf.Sqrt(desiredHeight * 2 * g);
        return initialV;
    }

    private float TimeForJump(float initialVelocity)
    {
        return 2 * initialVelocity / Physics.gravity.magnitude;
    }
}
