using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Allows for the oscillation of a point at angles around another point
/// </summary>
public class AngleOscillation : ILaserMovement
{

    private Vector3 _amplitudes = Vector3.zero;
    private Vector3 _speeds = Vector3.one;

    // Desinates whether we use a sine or cosine for osciallation
    private bool _xSine = true;
    private bool _ySine = false;
    private bool _zSine = false;

    // Controls which "direction" we are oscillating
    private bool _xInvert = false;
    private bool _yInvert = false;
    private bool _zInvert = false;

    // An offset in a group of lasers
    private float _offset = 0f; 

    public AngleOscillation()
    {

    }

    public Vector3 CalculatePosition(Vector3 pointBase, Vector3 otherBase, float time)
    {
        float length = Vector3.Distance(pointBase, otherBase);

        time += _offset;
        float inv = _xInvert ? -1 : 1;
        float x = _speeds.x * time * inv;
        x = _xSine ? Mathf.Sin(x) : Mathf.Cos(x);
        x = (_amplitudes.x * x);

        inv = _yInvert ? -1 : 1;
        float y = _speeds.y * time * inv;
        y = _ySine ? Mathf.Sin(y) : Mathf.Cos(y);
        y = (_amplitudes.y * y);

        inv = _zInvert ? -1 : 1;
        float z = _speeds.z * time * inv;
        z = _zSine ? Mathf.Sin(z) : Mathf.Cos(z);
        z = (_amplitudes.z * z);

        Vector3 final = RotatePointAroundPivot(pointBase, otherBase, new Vector3(x, y, z));
        return final;
    }

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Quaternion rotation = Quaternion.Euler(angles);
        return (rotation * (point - pivot)) + pivot;
    }

    #region Static Oscillation Contructors
    public static AngleOscillation MakeXArc(float xRange, float speed = .5f, bool invert = false, float offset = 0f)
    {
        AngleOscillation newData = new AngleOscillation();

        newData._amplitudes = new Vector3(xRange, 0, 0);
        newData._xSine = true;
        newData._xInvert = invert;
        newData._offset = offset;
        if (speed != 1) newData._speeds.x = speed;

        return newData;
    }

    public static AngleOscillation MakeYArc(float yRange, float speed = .5f, bool invert = false, float offset = 0f)
    {
        AngleOscillation newData = new AngleOscillation();

        newData._amplitudes = new Vector3(0, yRange, 0);
        newData._ySine = true;
        newData._yInvert = invert;
        newData._offset = offset;
        if (speed != 1) newData._speeds.y = speed;

        return newData;
    }

    public static AngleOscillation MakeZArc(float zRange, float speed = .5f, bool invert = false, float offset = 0f)
    {
        AngleOscillation newData = new AngleOscillation();

        newData._amplitudes = new Vector3(0, 0, zRange);
        newData._zSine = true;
        newData._zInvert = invert;
        newData._offset = offset;
        if (speed != 1) newData._speeds.z = speed;

        return newData;
    }
    #endregion // Static Oscillation Contructors
}
