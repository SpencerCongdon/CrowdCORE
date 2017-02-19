using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaveNode))]
[CanEditMultipleObjects]
public class WaveNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Start Wave"))
        {
            foreach (Object target in targets)
            {
                ((WaveNode)target).StartWave();
            }
        }
    }
}
