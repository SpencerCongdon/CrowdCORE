using System.Collections.Generic;
using UnityEngine;

public class LaserGenerator : MonoBehaviour
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

        // Place them along the X
        if(AlignX)
        {
            // We need the distance along the X that we are rendering
            float minX = transform.position.x - HalfSizeX;
            float maxX = transform.position.x + HalfSizeX;

            // All the values for controlling the extents of the lasers
            float startZ = transform.position.z - HalfSizeZ;
            float currentY = transform.position.y - HalfSizeY;
            float currentZ = startZ;
            float maxY = transform.position.y + HalfSizeY;
            float maxZ = transform.position.z + HalfSizeZ;

            // The vectors for the line
            Vector3 start = new Vector3(minX, 0, 0);
            Vector3 end = new Vector3(maxX, 0, 0);

            while(currentY <= maxY)
            {
                start.y = currentY;
                end.y = currentY;

                while (currentZ <= maxZ)
                {
                    start.z = currentZ;
                    end.z = currentZ;

                    // Create and place laser
                    Laser newLaser = Instantiate<Laser>(LaserPrefab);
                    newLaser.transform.parent = this.transform;
                    newLaser.SetLaser(start, end);

                    // Update values
                    // TODO: Randomize this
                    currentZ += MinSpacingZ;
                    
                }

                // TODO: Randomize this if necessary
                currentY += MinSpacingY;
                currentZ = startZ;
            }
        }

    }

    public void RemoveLasers()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        foreach (var child in children) DestroyImmediate(child);
        CrowdManager.Instance.CrowdCreated = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;
        Gizmos.DrawCube(transform.position, new Vector3(HalfSizeX * 2, HalfSizeY * 2, HalfSizeZ * 2));
    }
}
