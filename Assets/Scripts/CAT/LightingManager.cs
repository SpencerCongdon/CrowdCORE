using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : Singleton<LightingManager>
{
    public enum LightingEffect
    {
        Off,
        On,
        Strobe,
        Color
    }

    [System.Serializable]
    public struct LightingData
    {
        public Light light;
        public LightingEffect lightingEffect;
        public Color color;
    }

    [System.Serializable]
    public class LightingDataWrapper
    {
        public string DebugName;
        public float Length;
        public List<LightingData> Data;
    }

    [SerializeField]
    private float strobeTick = 0.1f;

    public List<LightingDataWrapper> Data;
    private int runningCount = 0;

    void Start()
    {
        StartCoroutine(RunLights());
    }

    private IEnumerator RunLights()
    {
        while (true)
        {
            for (int i = 0; i < Data.Count; i++)
            {
                while (runningCount > 0)
                {
                    yield return null;
                }
                yield return StartCoroutine(RunLightOperation(Data[i]));
            }
        }
    }

    private IEnumerator RunLightOperation(LightingDataWrapper data)
    {
        Debug.Log("Starting Lighting Operation: " + data.DebugName);
        runningCount = 0;
        for(int i = 0; i < data.Data.Count; i++)
        {
            switch (data.Data[i].lightingEffect)
            {
                case LightingEffect.Color:
                    Debug.Log("Changing color of light: " + data.Data[i].light.name);
                    runningCount++;
                    StartCoroutine(ChangeColor(data.Data[i], data.Length));
                    break;
                case LightingEffect.Off:
                case LightingEffect.On:
                    runningCount++;
                    Debug.Log("Turning on or off Light: " + data.Data[i].light.name);
                    StartCoroutine(OnOffLight(data.Data[i], data.Length));
                    break;
                case LightingEffect.Strobe:
                    runningCount++;
                    //Debug.Log("Strobing Light: " + data.Data[i].light.name);
                    StartCoroutine(StrobeLight(data.Data[i], data.Length));
                    break;
                default:
                    break;
            }
        }
        yield return new WaitForSeconds(data.Length);

    }

    private IEnumerator OnOffLight(LightingData data, float length)
    {
        bool status = true;
        bool originStatus = data.light.enabled;
        if (data.lightingEffect == LightingEffect.On)
        {
            status = true;
        }
        else if (data.lightingEffect == LightingEffect.Off)
        {
            status = false;
        }
        data.light.enabled = status;
        yield return new WaitForSeconds(length);
        data.light.enabled = originStatus;
        runningCount--;
    }

    private IEnumerator StrobeLight(LightingData data, float length)
    {
        float countLength = length;
        float strobeLength = strobeTick;

        while (countLength >= 0.0f)
        {
            data.light.enabled = false;
            strobeLength = strobeTick;
            while(strobeLength >= 0.0f)
            {
                yield return null;
                strobeLength -= Time.deltaTime;
                countLength -= Time.deltaTime;
            }
            data.light.enabled = true;
            strobeLength = strobeTick;
            while (strobeLength >= 0.0f)
            {
                yield return null;
                strobeLength -= Time.deltaTime;
                countLength -= Time.deltaTime;
            }
        }
        runningCount--;
    }

    private IEnumerator ChangeColor(LightingData data, float length)
    {
        Color originalColor = data.light.color;
        data.light.color = data.color;
        yield return new WaitForSeconds(length);
        data.light.color = originalColor;
        runningCount--;
    }
}