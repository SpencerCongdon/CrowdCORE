using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdInfluenceData
{
    private CrowdEnums.BehaviourType influence;
    public CrowdEnums.BehaviourType Influence { get { return influence; } }
    private CrowdMember influencer;
    public CrowdMember Influencer { get { return influencer; } set { influencer = value; } }
    private int influenceCount;
    public int Count { get { return influenceCount; } set { influenceCount = value; } }
    private float duration;
    public float Duration { get { return duration; } }

    public CrowdInfluenceData(CrowdEnums.BehaviourType i, CrowdMember crowdie, int count, float newDuration)
    {
        influence = i;
        influencer = crowdie;
        influenceCount = count;
        duration = newDuration;
    }
}
