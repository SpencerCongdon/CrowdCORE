using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SurferMind))]
public class SurferMindEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Change direction"))
        {
            ((SurferMind)target).ChangeDirection();
        }
    }
}