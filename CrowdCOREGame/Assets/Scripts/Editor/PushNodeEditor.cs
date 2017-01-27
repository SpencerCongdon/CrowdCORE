using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PushNode))]
[CanEditMultipleObjects]
public class PushNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("PUSH"))
        {
            foreach (Object target in targets)
            {
                ((PushNode)target).PushEveryone();
            }
        }
    }
}