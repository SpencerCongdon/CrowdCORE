using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdSpawner : MonoBehaviour {

    static Color gizmoColour = new Color(0.0f, 0.9f, 0.2f, 0.4f);

    public float CrowdSizeX = 10f;
    public float CrowdSizeZ = 10f;
    public float MinSpacing = 0f;

    public GameObject MemberPrefab;
    public BoxCollider ExitBox; // The box that determines if someone is still in the crowd

    [SerializeField]
    private List<Transform> mLookLocations;

    // Use this for initialization
    void Start ()
    {
        if(MemberPrefab != null)
        {
            SpawnCrowd();
        }
        else
        {
            Debug.LogWarning(this.name + " does not have a valid memberPrefab assigned.");
        }
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void SpawnNewCrowd()
    {
        RemoveCrowd();
        SpawnCrowd();
    }

    void SpawnCrowd()
    {
        // Check to make sure we don't have an existing crowd
        if(this.transform.childCount > 0)
        {
            Debug.LogWarning("Cannot spawn crowd when spawner already has child objects");
            return;
        }

        GameObject firstMember = Instantiate(MemberPrefab, this.transform.position, new Quaternion()) as GameObject;

        Renderer rend = firstMember.GetComponentInChildren<Renderer>();

        if(rend != null)
        {
            float memberX = rend.bounds.extents.x * 2f + MinSpacing;
            float memberZ = rend.bounds.extents.z * 2f + MinSpacing;

            //Debug.Log("SpawnCrowd() Members have bounds of " + memberX + "x and " + memberZ + "z");

            int membersPerX = (int)(CrowdSizeX / memberX);
            int membersPerZ = (int)(CrowdSizeZ / memberZ);

            //Debug.Log("SpawnCrowd() Number of members: " + membersPerX + "x and " + membersPerZ + "z");

            float xSpacing = CrowdSizeX / membersPerX;
            float zSpacing = CrowdSizeZ / membersPerZ;

            //Debug.Log("SpawnCrowd() Spacing: " + xSpacing + "x and " + zSpacing + "z");

            DestroyImmediate(firstMember);

            PlaceMembers(membersPerX, membersPerZ, xSpacing, zSpacing);

            CrowdManager.Instance.CrowdCreated = true;
        }
        else
        {
            Debug.LogWarning("CrowdSpawner MemberPrefab does not have a renderer.");
        }
    }

    void PlaceMembers(int membersPerX, int membersPerZ, float xSpacing, float zSpacing)
    {
        int numPlaced = 0;
        Vector3 spawnCenter = this.transform.position;
        float halfX = CrowdSizeX * .5f;
        float halfZ = CrowdSizeZ * .5f;
        float startPosX = spawnCenter.x - halfX + (zSpacing * .5f);
        float currentPosX = startPosX;
        float currentPosZ = spawnCenter.z - halfZ + (zSpacing * .5f);

        float upperBoundsX = spawnCenter.x + halfX;
        float lowerBoundsX = spawnCenter.x - halfX;
        float upperBoundsZ = spawnCenter.z + halfZ;
        float lowerBoundsZ = spawnCenter.z - halfZ;

        for (var zIt = 0; zIt < membersPerZ; zIt++ )
        {
            for (var xIt = 0; xIt < membersPerX; xIt++)
            {
                GameObject newCrowdie = Instantiate(MemberPrefab, new Vector3(currentPosX, 0.0f, currentPosZ), new Quaternion()) as GameObject;
                newCrowdie.transform.parent = this.transform;

                if (mLookLocations.Count > 0)
                {
                    int randTargetIndex = Random.Range(0, mLookLocations.Count);
                    newCrowdie.transform.LookAt(
                        new Vector3(mLookLocations[randTargetIndex].position.x,
                        newCrowdie.transform.position.y,
                        mLookLocations[randTargetIndex].position.z
                        ));
                }

                CrowdMember newMember = newCrowdie.GetComponent<CrowdMember>();
                newMember.SetCrowdBounds(spawnCenter, lowerBoundsX, upperBoundsX, lowerBoundsZ, upperBoundsZ);

                // TODO: Do we need this list?
                //if (CrowdManager.Instance != null) CrowdManager.Instance.AddCrowdie(newMember);

                // Update to new position
                currentPosX += xSpacing;
                numPlaced++;
            }

            // Start at the other side again
            currentPosX = startPosX;
            currentPosZ += zSpacing;
        }

        Debug.Log("CrowdSpawer spawed " + numPlaced + " members");
        Debug.Assert(numPlaced == (membersPerX * membersPerZ), "Placed: " + numPlaced + " Expected: " + (membersPerX * membersPerZ));
    }

    public void RemoveCrowd()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        foreach (var child in children) DestroyImmediate(child);
        CrowdManager.Instance.CrowdCreated = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;
        Gizmos.DrawCube(transform.position, new Vector3(CrowdSizeX, .2f, CrowdSizeZ));
    }
}
