using UnityEngine;

public class RippleNode : MonoBehaviour
{
    private const float NODE_CENTER_RADIUS = 0.2f;  // For the gizmo

    public float radius;            // The sphere of influence for the ripple
    public float minHeight;         // Height we want people to jump
    public float maxHeight;         // Factor by which jumps will vary
    public float waveDuration;      // Time that the wave is active
    public float waveSpeed;         // Modifies how fast the jump propegates through the crowd
    public bool imposeDelay;        // Whether or not the jumper has to wait, or can jump on first interval

    /// <summary>
    /// Initiate a wave at the current location
    /// </summary>
    public void StartWave()
    {
        Vector3 nodePos = this.transform.position;
        int layerMask = 1 << LayerMask.NameToLayer("CrowdMember");

        Collider[] hitColliders = Physics.OverlapSphere(nodePos, radius, layerMask);
        foreach (Collider c in hitColliders)
        {
            CrowdMember jumper = c.GetComponent<CrowdMember>();
            Debug.Assert(jumper != null, "GameObject on CrowdMember layer doesn't have a CrowdMember script");
       
            if(jumper != null)
            {
                Vector3 collPos = c.transform.position;
                collPos.y = nodePos.y;

                float distanceToOrigin = (nodePos - collPos).magnitude;
                float delay = waveSpeed * distanceToOrigin;

                jumper.JoinRipple(delay, waveDuration, minHeight, maxHeight, imposeDelay);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Values used throughout the drawing
        Vector3 nodePos = this.transform.position;

        // Draw the wave origin line
        Gizmos.color = new Color(1f, 0f, 0f);
        Gizmos.DrawWireSphere(nodePos, NODE_CENTER_RADIUS);

        // Draw the circle for the ripple area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(nodePos, radius);

        // Draw the distances to crowd members influenced by the wave
        Gizmos.color = Color.yellow;
        int layerMask = 1 << LayerMask.NameToLayer("CrowdMember");
        Collider[] hitColliders = Physics.OverlapSphere(nodePos, radius, layerMask);
        foreach (Collider c in hitColliders)
        {
            Vector3 collPos = c.transform.position;
            collPos.y = nodePos.y;  // So that the lines are horizontal
            Gizmos.DrawLine(collPos, nodePos);
        }
    }
}
