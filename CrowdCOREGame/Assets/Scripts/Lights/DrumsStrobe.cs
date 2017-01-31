using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumsStrobe : MonoBehaviour
{
    private float timer = 0.0f;
    public float randomMin = 0.5f;
    public float randomMax = 5.0f;
    private float randomTime = 0.0f;
    public Light drumLight;

    void Start ()
    {
        drumLight = gameObject.GetComponent<Light>();

        if (drumLight != null)
            drumLight.enabled = false;

        randomTime = Random.Range(randomMin, randomMax);
    }
    void Update ()
    {
        if (drumLight != null)
        {
            timer += Time.deltaTime;

            if (timer > randomTime)
            {
                drumLight.enabled = !drumLight.enabled;
                timer = 0.0f;
                randomTime = Random.Range(randomMin, randomMax);
            }
        }
    }
}
