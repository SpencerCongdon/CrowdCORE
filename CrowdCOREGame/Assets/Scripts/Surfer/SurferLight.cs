using UnityEngine;

/// <summary>
/// A light that follows and highlights a Surfer 
/// </summary>
public class SurferLight : MonoBehaviour
{
    public Surfer followSurfer;
    public float offset = 1.0f;

    void Start ()
    {

    }

	void Update ()
    {
        if(followSurfer!= null)
        {
            this.transform.position = new Vector3(followSurfer.transform.position.x,
                                      followSurfer.transform.position.y + offset,
                                      followSurfer.transform.position.z);
        }
    }
}
