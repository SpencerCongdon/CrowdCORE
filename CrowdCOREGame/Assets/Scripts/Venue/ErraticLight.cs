using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErraticLight : MonoBehaviour
{
    private Vector3 homePosition;
    private float startY;
    public float minX = 0.0f;
    public float maxX = 0.0f;
    public float minZ = 0.0f;
    public float maxZ = 0.0f;
    private float randX;
    private float randZ;

    private float timer = 0.0f;
    public float updateRate = 0.01f;

    void Awake()
    {
        homePosition = gameObject.transform.position;
        startY = homePosition.y;
    }

    void Start ()
    {
    }

    void Update ()
    {
        timer += Time.deltaTime;
        if (timer > updateRate)
        {
            randX = Random.Range(minX, maxX);
            randZ = Random.Range(minZ, maxZ);
            gameObject.transform.position = new Vector3(randX, startY, randZ);
            timer = 0.0f;
        }
    }
}
