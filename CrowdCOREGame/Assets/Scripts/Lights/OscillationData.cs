
using UnityEngine;

public class OscillationData
{
    private Vector3 _amplitudes = Vector3.zero;
    private Vector3 _speeds = Vector3.one;
    private bool _xSine = true;
    private bool _ySine = false;
    private bool _zSine = false;
    private bool _xInvert = false;
    private bool _yInvert = false;
    private bool _zInvert = false;
    private float _offset = 0f;

    public OscillationData()
    {

    }

    public OscillationData(Vector3 mag, float speed = 1)
    {
        _amplitudes = mag;
    }

    public OscillationData(Vector3 newMagnitues, Vector3 newSpeeds, bool newXSine, bool newYSine, bool newZSine)
    {
        _amplitudes = newMagnitues;
        _speeds = newSpeeds;
        _xSine = newXSine;
        _ySine = newYSine;
        _zSine = newZSine;
    }

    public Vector3 CalculatePos(Vector3 origin,  float time)
    {
        time += _offset;
        float inv = _xInvert ? -1 : 1;
        float x = _speeds.x * time * inv;
        x = _xSine ? Mathf.Sin(x) : Mathf.Cos(x);
        x = origin.x + (_amplitudes.x * x);

        inv = _yInvert ? -1 : 1;
        float y = _speeds.y * time * inv;
        y = _ySine ? Mathf.Sin(y) : Mathf.Cos(y);
        y = origin.y + (_amplitudes.y * y);

        inv = _zInvert ? -1 : 1;
        float z = _speeds.z * time * inv;
        z = _zSine ? Mathf.Sin(z) : Mathf.Cos(z);
        z = origin.z + (_amplitudes.z * z);

        return new Vector3(x, y, z);
    }

    public static OscillationData MakeXLine(float xMag, float speed = 1, bool invert = false, float offset = 0f)
    {
        OscillationData newData = new OscillationData();

        newData._amplitudes = new Vector3(xMag, 0, 0);
        newData._xSine = true;
        newData._xInvert = invert;
        newData._offset = offset;
        if (speed != 1) newData._speeds.x = speed;

        return newData;
    }

    public static OscillationData MakeYLine(float yMag, float speed = 1, bool invert = false, float offset = 0f)
    {
        OscillationData newData = new OscillationData();

        newData._amplitudes = new Vector3(0, yMag, 0);
        newData._ySine = true;
        newData._yInvert = invert;
        newData._offset = offset;
        if (speed != 1) newData._speeds.y = speed;

        return newData;
    }

    public static OscillationData MakeZLine(float zMag, float speed = 1, bool invert = false, float offset = 0f)
    {
        OscillationData newData = new OscillationData();

        newData._amplitudes = new Vector3(0, 0, zMag);
        newData._zSine = true;
        newData._zInvert = invert;
        newData._offset = offset;
        if (speed != 1) newData._speeds.z = speed;

        return newData;
    }

    public static OscillationData MakeXYCircle(float xMag, float yMag, float speed = 1, bool invert = false, float offset = 0f)
    {
        OscillationData newData = new OscillationData();

        newData._amplitudes = new Vector3(xMag, yMag, 0);
        newData._xSine = true;
        newData._ySine = false;
        newData._xInvert = invert;
        newData._yInvert = invert;
        newData._offset = offset;
        if (speed != 1)
        {
            newData._speeds = new Vector3(speed, speed, speed);
        }

        return newData;
    }

    public static OscillationData MakeYZCircle(float yMag, float zMag, float speed = 1, bool invert = false, float offset = 0f)
    {
        OscillationData newData = new OscillationData();

        newData._amplitudes = new Vector3(0, yMag, zMag);
        newData._ySine = true;
        newData._zSine = false;
        newData._yInvert = invert;
        newData._zInvert = invert;
        newData._offset = offset;

        if (speed != 1)
        {
            newData._speeds = new Vector3(speed, speed, speed);
        }

        return newData;
    }

    public static OscillationData MakeXZCircle(float xMag, float zMag, float speed = 1, bool invert = false, float offset = 0f)
    {
        OscillationData newData = new OscillationData();

        newData._amplitudes = new Vector3(xMag, 0, zMag);
        newData._xSine = true;
        newData._zSine = false;
        newData._xInvert = invert;
        newData._zInvert = invert;
        newData._offset = offset;
        if (speed != 1)
        {
            newData._speeds = new Vector3(speed, speed, speed);
        }

        return newData;
    }
}
