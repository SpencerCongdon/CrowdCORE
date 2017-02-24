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

        if (GUILayout.Button("Start Surge"))
        {
            CrowdMember crowdie = (CrowdMember)target;
            CrowdInfluenceData data = new CrowdInfluenceData(CrowdEnums.BehaviourType.SurgeForward, crowdie, 10, 5f);
            crowdie.StartInfluence(data);
        }

        if (GUILayout.Button("Start Mosh"))
        {
            CrowdMember crowdie = (CrowdMember)target;
            CrowdInfluenceData data = new CrowdInfluenceData(CrowdEnums.BehaviourType.Mosh, crowdie, 10, 5f);
            crowdie.StartInfluence(data);
        }

        if (GUILayout.Button("Start Jump Together"))
        {
            CrowdMember crowdie = (CrowdMember)target;
            CrowdInfluenceData data = new CrowdInfluenceData(CrowdEnums.BehaviourType.JumpTogether, crowdie, 10, 5f);
            crowdie.StartInfluence(data);
        }
    }
}