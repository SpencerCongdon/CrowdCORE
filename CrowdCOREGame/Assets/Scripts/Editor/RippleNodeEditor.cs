using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RippleNode))]
[CanEditMultipleObjects]
public class RippleNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Start Ripple"))
        {
            foreach (Object target in targets)
            {
                ((RippleNode)target).StartWave();
            }
        }
    }
}
