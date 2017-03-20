using System.Collections;
using UnityEngine;

/// <summary>
/// It's a laser
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    private Coroutine flickerRoutine;

    [SerializeField]
    private LineRenderer line;

    [SerializeField]
    private bool flicker = false;
    public float minOnInterval = .4f;
    public float maxOnInterval = 2f;
    public float minOffInterval = .4f;
    public float maxOffInterval = 2f;

    public float minWidth = .1f;
    public float maxWidth = .4f;
    public float widthTime = 0f;
    public float widthDuration = 1f;
    public bool isGrowing = true;

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
        if(flickerRoutine == null)
        {
            flickerRoutine = StartCoroutine(Flicker());
        }

        bool switchGrowth = (isGrowing) ? widthTime > widthDuration : widthTime < 0;
        if (switchGrowth) isGrowing = !isGrowing;

        widthTime += (isGrowing) ? Time.deltaTime : -Time.deltaTime;
        float progress = widthTime / widthDuration;
        progress = Mathf.Lerp(minWidth, maxWidth, progress);
        line.startWidth = progress;
        line.endWidth = progress;
    }

    public void SetLaser(Vector3 start, Vector3 end)
    {
        line.SetPosition(0, start);
        line.SetPosition(1, end);
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
