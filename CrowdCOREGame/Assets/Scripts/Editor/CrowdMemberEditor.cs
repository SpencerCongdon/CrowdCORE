using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CrowdMember))]
[CanEditMultipleObjects]
public class CrowdMemberEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}