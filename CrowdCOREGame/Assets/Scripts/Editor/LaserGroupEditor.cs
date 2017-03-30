using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LaserGroup))]
public class LaserGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Regenerate Group"))
        {
            ((LaserGroup)target).SpawnNewLasers();
        }
        if (GUILayout.Button("Remove Lasers"))
        {
            ((LaserGroup)target).RemoveLasers();
        }
        if (GUILayout.Button("Start Oscillation"))
        {
            ((LaserGroup)target).OscillateLasers();
        }
    }
}