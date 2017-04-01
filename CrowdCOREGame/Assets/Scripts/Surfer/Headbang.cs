using UnityEngine;

public class Headbang : MonoBehaviour
{
    private float lerpTime = 0.0f;
    private Quaternion start;
    private Quaternion end;

    public float minRot     = -45.0f;
    public float maxRot     = 45.0f;
    public float rotSpeed   = 2.0f;

    // Use this for initialization
    void Start ()
    {
        start = transform.localRotation * Quaternion.Euler(transform.right * minRot);
        end = transform.localRotation * Quaternion.Euler(transform.right * maxRot);
    }
	
	// Update is called once per frame
	void Update ()
    {
        lerpTime = Mathf.PingPong(Time.time * rotSpeed, 1.0f);
        transform.localRotation = Quaternion.Slerp(start, end, lerpTime);
    }
}
