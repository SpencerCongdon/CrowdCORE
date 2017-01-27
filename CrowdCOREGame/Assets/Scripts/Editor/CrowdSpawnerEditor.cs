using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CrowdSpawner))]
public class CrowdSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Respawn Crowd"))
        {
            ((CrowdSpawner)target).SpawnNewCrowd();
        }
        if (GUILayout.Button("Remove Crowd"))
        {
            ((CrowdSpawner)target).RemoveCrowd();
        }
    }
}