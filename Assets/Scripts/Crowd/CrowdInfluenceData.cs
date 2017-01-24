using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdInfluenceData
{
    private CrowdEnums.InfluenceType influence;
    public CrowdEnums.InfluenceType Influence { get { return influence; } }
    private CrowdMember influencer;
    public CrowdMember Influencer { get { return influencer; } set { influencer = value; } }
    private int influenceCount;
    public int Count { get { return influenceCount; } set { influenceCount = value; } }

    public CrowdInfluenceData(CrowdEnums.InfluenceType i, CrowdMember crowdie, int count)
    {
        influence = i;
        influencer = crowdie;
        influenceCount = count;
    }
}
