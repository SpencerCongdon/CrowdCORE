using System.Collections;
using UnityEngine;

/// <summary>
/// It's a laser
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    public enum OscillationType
    {
        StartOnly,
        EndOnly,
        Together,   // Both ends
        Opposite    // Both ends
    }

    private Coroutine flickerRoutine;

    [SerializeField]
    private LineRenderer line;

    // Base line dimensions
    private Vector3 startBase;
    private Vector3 endBase;

    // Flickering
    [SerializeField]
    private bool isFlickering = false;
    public float minOnInterval = .4f;
    public float maxOnInterval = 2f;
    public float minOffInterval = .4f;
    public float maxOffInterval = 2f;

    // Pulsing
    public bool isPulsing = true;
    public float minWidth = .1f;
    public float maxWidth = .4f;
    public float widthTime = 0f;
    public float widthDuration = 1f;
    public bool isGrowing = true;

    // Ocillation
    [SerializeField]
    private bool isOscillating = false;
    private OscillationData startOscillation;
    private OscillationData endOscillation;

    // Colour
    private Color startColour = Color.white;
    private Color endColour = Color.white;

    private Color colour0 = Color.white;
    private Color colour1 = Color.white;

    public Vector3 StartBase { get { return startBase; } }
    public Vector3 EndBase { get { return endBase; } }

    // Use this for initialization
    void Start()
    {
        if(line == null)
        {
            line = GetComponent<LineRenderer>();
        }

        Debug.Assert(line != null, this.name + " needs a line renderer");

        line.startWidth = minWidth;
        line.endWidth = minWidth;
        widthTime = Random.Range(0, widthDuration); // Start at a random time, giving us a random width
    }

    // Update is called once per frame
    void Update()
    {
        if(isFlickering && flickerRoutine == null)
        {
            flickerRoutine = StartCoroutine(Flicker());
        }

        if (isPulsing) UpdatePulse();
        if (isOscillating) UpdateOscillation();
        UpdateColour();
    }

    private void UpdatePulse()
    {
        bool switchGrowth = (isGrowing) ? widthTime > widthDuration : widthTime < 0;
        if (switchGrowth) isGrowing = !isGrowing;

        widthTime += (isGrowing) ? Time.deltaTime : -Time.deltaTime;
        float progress = widthTime / widthDuration;
        progress = Mathf.Lerp(minWidth, maxWidth, progress);
        line.startWidth = progress;
        line.endWidth = progress;
    }

    private void UpdateOscillation()
    {
        float theTime = Time.realtimeSinceStartup;

        if(startOscillation != null)
        {
            line.SetPosition(0, startOscillation.CalculatePos(startBase, theTime));
        }

        if (endOscillation != null)
        {
            line.SetPosition(1, endOscillation.CalculatePos(endBase, theTime));
        }
    }

    private void UpdateColour()
    {
        line.startColor = Color.Lerp(colour0, colour1, Mathf.PingPong(Time.time, 1));
        line.endColor = Color.Lerp(colour1, colour0, Mathf.PingPong(Time.time, 1));
    }

    public void SetEndpoints(Vector3 start, Vector3 end)
    {
        startBase = start;
        endBase = end;
        line.SetPosition(0, startBase);
        line.SetPosition(1, endBase);
    }

    public void SetOscillation(OscillationData start, OscillationData end)
    {
        startOscillation = start;
        endOscillation = end;
        isOscillating = (start != null || end != null);
    }

    public void SetColour(Color first, Color second)
    {
        colour0 = first;
        colour1 = second;
        line.startColor = first;
        line.endColor = second;
    }

    public void StopOscillation()
    {
        startOscillation = endOscillation = null;
        isOscillating = false;
    }

    private IEnumerator Flicker()
    {
        yield return new WaitForSeconds(Random.Range(minOnInterval, maxOnInterval));
        line.enabled = false;
        yield return true;
        line.enabled = true;
        flickerRoutine = null;
    }

    private IEnumerator Flash()
    {
        yield return new WaitForSeconds(Random.Range(minOnInterval, maxOnInterval));
        line.enabled = false;
        yield return new WaitForSeconds(Random.Range(minOffInterval, maxOffInterval));
        line.enabled = true;
        flickerRoutine = null;
    }

}
