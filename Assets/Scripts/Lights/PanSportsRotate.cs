using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanSportsRotate : MonoBehaviour
{
    public float rotationSpeed = 1.0f;

    void Update()
    {
        // Rotate the object around its local X axis at 1 degree per second
        transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);

        // ...also rotate around the World's Y axis
        //transform.Rotate(Vector3.up, Time.deltaTime, Space.World);
    }
}
