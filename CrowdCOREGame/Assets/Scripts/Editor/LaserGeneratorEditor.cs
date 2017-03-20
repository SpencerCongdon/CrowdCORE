using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LaserGenerator))]
public class LaserGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Respawn Crowd"))
        {
            ((LaserGenerator)target).SpawnNewLasers();
        }
        if (GUILayout.Button("Remove Crowd"))
        {
            ((LaserGenerator)target).RemoveLasers();
        }
    }
}