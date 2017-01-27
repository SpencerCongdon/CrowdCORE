using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeholder : MonoBehaviour
{
    public float rand;
    GameObject thisGameObject;
    public float max = 2.0f;
    public float min = 0.0f;

    void Start()
    {
        thisGameObject = this.gameObject;
    }
    void Update ()
    {
        rand = Random.Range(min, max);
        thisGameObject.transform.position = new Vector3 (thisGameObject.transform.position.x, rand, thisGameObject.transform.position.z);
    }
}
