using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdEnums
{
    public enum InfluenceType
    {
        None = -1,      // Crowd member is not going to jump
        Normal,         // We are just jumping
        Influencer,
        WaveJump,
        NewWave,
        JumpTogether,
        Mosh,
        SurgeForward,
        Count
    }
}
