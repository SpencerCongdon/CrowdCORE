using UnityEngine;

/// <summary>
/// Makes decisions for an CPU surfer
/// </summary>
[RequireComponent(typeof(SurferControl))]
public class SurferMind : MonoBehaviour
{
    [SerializeField]
    private SurferControl control;

    // The extents that govern where the surfer will surf
    [SerializeField] private float defaultWanderExtent;
    [SerializeField] private float xWanderMax;
    [SerializeField] private float xWanderMin;
    [SerializeField] private float zWanderMax;
    [SerializeField] private float zWanderMin;
    [SerializeField] private Vector3 crowdCenter;

    [SerializeField] private bool isPositiveX;
    [SerializeField] private bool isPositiveZ;
    [SerializeField] private bool isNearXEdge;
    [SerializeField] private bool isNearZEdge;
    [SerializeField] private float boundaryThreshold;

    [SerializeField] private float currentXmove;
    [SerializeField] private float currentZmove;

    [SerializeField] private float maxMove;
    [SerializeField] private float minMove;

    [SerializeField] private bool canPunch;
    [SerializeField] private bool canKick;

    void Start()
    {

        if (control == null)
        {
            control = GetComponent<SurferControl>();
        }

        // TODO: Handle multiple crowd spawners
        CrowdSpawner spawner =  GameObject.FindGameObjectWithTag(Tag.CROWD_SPAWNER).GetComponent<CrowdSpawner>();

        if(spawner != null)
        {
            crowdCenter = spawner.transform.position;

            // The ranges are half of the crowd size
            float xExtent = spawner.CrowdSizeX * .5f;
            float zExtent = spawner.CrowdSizeZ * .5f;

            xWanderMax = crowdCenter.x + xExtent;
            xWanderMin = crowdCenter.x - xExtent;
            zWanderMax = crowdCenter.z + zExtent;
            zWanderMin = crowdCenter.z - zExtent;
        }
        else
        {
            xWanderMax = transform.position.x + defaultWanderExtent;
            xWanderMin = transform.position.x - defaultWanderExtent;
            zWanderMax = transform.position.z + defaultWanderExtent;
            zWanderMin = transform.position.z - defaultWanderExtent;
        }

        ChangeDirection();
        
    }

    void Update()
    {
        isPositiveX = transform.position.x > crowdCenter.x;
        isPositiveZ = transform.position.z > crowdCenter.z;

        float xEdge = isPositiveX ? xWanderMax : xWanderMin;
        float zEdge = isPositiveZ ? zWanderMax : zWanderMin;

        bool wasNearX = isNearXEdge;
        bool wasNearZ = isNearZEdge;
        
        isNearXEdge = Mathf.Abs(xEdge - transform.position.x) < boundaryThreshold;
        isNearZEdge = Mathf.Abs(zEdge - transform.position.z) < boundaryThreshold;

        // If we've arrived near the edge, let's change course
        if ((!wasNearX && isNearXEdge) || (!wasNearZ && isNearZEdge))
        {
            ChangeDirection();
        }

        control.PerformMovement(currentXmove, currentZmove);


        if (canPunch)
        {
            // TODO: Decide if we should punch
        }

        if(canKick)
        {
            // TODO: Decide if we should kick
        }

    }

    public void ChangeDirection()
    {
        currentXmove = GetMoveInput(isPositiveX, isNearXEdge);
        currentZmove = GetMoveInput(isPositiveZ, isNearZEdge);
    }

    private float GetMoveInput(bool isPositive, bool isNearEdge)
    {
        float input = 0;
        if(isNearEdge)
        {
            // Force them to try and stay in
            input = isPositive ? -maxMove : maxMove;
        }
        else
        {
            input = Random.Range(-maxMove, maxMove);
        }
        return input;
    }

    #region Debugging
    void OnDrawGizmos()
    {
        float yPos = transform.position.y;

        // Draw the crowd boundary
        Vector3 cor1 = new Vector3(xWanderMax, yPos, zWanderMax);
        Vector3 cor2 = new Vector3(xWanderMax, yPos, zWanderMin);
        Vector3 cor3 = new Vector3(xWanderMin, yPos, zWanderMin);
        Vector3 cor4 = new Vector3(xWanderMin, yPos, zWanderMax);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(cor1, cor2);
        Gizmos.DrawLine(cor2, cor3);
        Gizmos.DrawLine(cor3, cor4);
        Gizmos.DrawLine(cor4, cor1);

        // Draw the "edge" threshold
        cor1 = new Vector3(xWanderMax - boundaryThreshold, yPos, zWanderMax - boundaryThreshold);
        cor2 = new Vector3(xWanderMax - boundaryThreshold, yPos, zWanderMin + boundaryThreshold);
        cor3 = new Vector3(xWanderMin + boundaryThreshold, yPos, zWanderMin + boundaryThreshold);
        cor4 = new Vector3(xWanderMin + boundaryThreshold, yPos, zWanderMax - boundaryThreshold);

        Gizmos.color = new Color(1f, .8f, 0f);
        Gizmos.DrawLine(cor1, cor2);
        Gizmos.DrawLine(cor2, cor3);
        Gizmos.DrawLine(cor3, cor4);
        Gizmos.DrawLine(cor4, cor1);
    }
    #endregion // Debugging
}
