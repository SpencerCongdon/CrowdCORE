using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CrowdGuy))]
[CanEditMultipleObjects]
public class CrowdGuyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("PUSH"))
        {
            foreach(Object target in targets)
            {
                ((CrowdGuy)target).PushAside();
            }
        }
    }
}