using System.Collections.Generic;
using UnityEngine;

public class LaserGroup : MonoBehaviour
{
    static Color gizmoColour = new Color(0.0f, 0.9f, 0.2f, 0.4f);

    // The area to fill
    public float HalfSizeX = 10f;
    public float HalfSizeY = 10f;
    public float HalfSizeZ = 10f;

    public float MinSpacingX = 2f;
    public float MinSpacingY = 2f;
    public float MinSpacingZ = 2f;

    public bool RandomSpacing = false;

    // Whether or not to draw each axis
    public bool AlignX = true;
    public bool AlignY = false;
    public bool AlignZ = false;

    public Laser LaserPrefab;

    public Color first = Color.white;
    public Color second = Color.white;

    public float xMag = 0f;
    public float yMag = 0f;
    public float zMag = 0f;

    private List<Laser> lasers = new List<Laser>();

    // Use this for initialization
    void Start ()
    {
        if(LaserPrefab != null)
        {
            GenerateLasers();
        }
        else
        {
            GameLog.LogWarning(this.name + " does not have a valid LaserPrefab assigned.", GameLog.Category.Lighting);
        }
    }
    
    // Update is called once per frame
    void Update () {
    }

    public void SpawnNewLasers()
    {
        RemoveLasers();
        GenerateLasers();
    }

    void GenerateLasers()
    {
        // Check to make sure we don't have an existing crowd
        if(this.transform.childCount > 0)
        {
            GameLog.LogWarning("Cannot spawn lasers when spawner already has child objects", GameLog.Category.Lighting);
            return;
        }

        // Make sure we are only generating for one dimension
        CheckAlignments();

        // We need the distance along the X that we are rendering
        float minExtent, maxExtent;
        float minDim1, maxDim1;
        float minDim2, maxDim2;
        float currDim1, currDim2;
        minExtent = maxExtent = minDim1 = maxDim1 = 
            minDim2 = maxDim2 = currDim1 = currDim2 = 0;

        Vector3 start = new Vector3();
        Vector3 end = new Vector3();

        // Place them along the X
        if (AlignX)
        {
            // X is the extents
            minExtent = transform.position.x - HalfSizeX;
            maxExtent = transform.position.x + HalfSizeX;

            // Y is dimension one
            minDim1 = transform.position.y - HalfSizeY;
            maxDim1 = transform.position.y + HalfSizeY;

            // Z is dimension two
            minDim2 = transform.position.z - HalfSizeZ;
            maxDim2 = transform.position.z + HalfSizeZ;

            start = new Vector3(minExtent, 0, 0);
            end = new Vector3(maxExtent, 0, 0);
        }
        else if (AlignY)
        {
            // X is dimension one
            minDim1 = transform.position.x - HalfSizeX;
            maxDim1 = transform.position.x + HalfSizeX;

            // Y is the extents
            minExtent = transform.position.y - HalfSizeY;
            maxExtent = transform.position.y + HalfSizeY;

            // Z is dimension two
            minDim2 = transform.position.z - HalfSizeZ;
            maxDim2 = transform.position.z + HalfSizeZ;

            start = new Vector3(0, minExtent, 0);
            end = new Vector3(0, maxExtent, 0);
        }
        else
        {
            // X is dimension one
            minDim1 = transform.position.x - HalfSizeX;
            maxDim1 = transform.position.x + HalfSizeX;

            // Y is dimension two
            minDim2 = transform.position.y - HalfSizeY;
            maxDim2 = transform.position.y + HalfSizeY;

            // Z is the extents
            minExtent = transform.position.z - HalfSizeZ;
            maxExtent = transform.position.z + HalfSizeZ;

            start = new Vector3(0, 0, minExtent);
            end = new Vector3(0, 0, maxExtent);
        }

        currDim1 = minDim1;
        currDim2 = minDim2;
        

        while (currDim1 <= maxDim1)
        {
            if(AlignX)
            {
                start.y = currDim1;
                end.y = currDim1;
            }
            else
            {
                start.x = currDim1;
                end.x = currDim1;
            }

            while (currDim2 <= maxDim2)
            {
                if (AlignZ)
                {
                    start.y = currDim2;
                    end.y = currDim2;
                }
                else
                {
                    start.z = currDim2;
                    end.z = currDim2;
                }

                // Create and place laser
                Laser newLaser = Instantiate<Laser>(LaserPrefab);
                newLaser.transform.parent = this.transform;

                newLaser.SetEndpoints(start, end);
                newLaser.SetColour(first, second);
                lasers.Add(newLaser);

                // Update values
                // TODO: Randomize this
                currDim2 += MinSpacingZ;
            }

            // TODO: Randomize this if necessary
            currDim1 += MinSpacingY;
            currDim2 = minDim2;
        }

    }

    public void RemoveLasers()
    {
        foreach (Laser l in lasers) DestroyImmediate(l.gameObject);
        lasers.Clear();
        CrowdManager.Instance.CrowdCreated = false;
    }

    /// <summary>
    /// Currently we don't allow generation of more than one alignment at a time
    /// Make sure only one is active at a time
    /// </summary>
    public void CheckAlignments()
    {
        // Prefer in x, y, z order
        if(AlignX)
        {
            AlignY = false;
            AlignZ = false;
        }
        else if(AlignY)
        {
            AlignZ = false;
        }
    }

    public void OscillateLasers()
    {
        // Generate one oscillation
        

        Vector3 lowBase = lasers[0].StartBase;
        foreach (Laser l in lasers)
        {
            float startoffset = (l.StartBase - lowBase).magnitude;

            OscillationData start = OscillationData.MakeYLine(yMag, 1f, invert: false, offset: startoffset);
            OscillationData end = OscillationData.MakeYLine(yMag, 1f, invert: true, offset: startoffset);
            //OscillationData start = OscillationData.MakeYZCircle(yMag, zMag, 1.5f, invert: false, offset: startoffset);
            //OscillationData end = OscillationData.MakeYZCircle(yMag, zMag, 1.5f, invert: true, offset: startoffset);
            l.SetOscillation(start, end);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;
        Gizmos.DrawWireCube(transform.position, new Vector3(HalfSizeX * 2, HalfSizeY * 2, HalfSizeZ * 2));
    }
}
