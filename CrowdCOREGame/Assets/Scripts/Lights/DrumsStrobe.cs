using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumsStrobe : MonoBehaviour
{
    private float timer = 0.0f;
    public float randomMin = 0.5f;
    public float randomMax = 5.0f;
    private float randomTime = 0.0f;
    public Light light;

    void Start ()
    {
        light = gameObject.GetComponent<Light>();

        if (light != null)
            light.enabled = false;

        randomTime = Random.Range(randomMin, randomMax);
    }
    void Update ()
    {
        if (light != null)
        {
            timer += Time.deltaTime;

            if (timer > randomTime)
            {
                light.enabled = !light.enabled;
                timer = 0.0f;
                randomTime = Random.Range(randomMin, randomMax);
            }
        }
    }
}
