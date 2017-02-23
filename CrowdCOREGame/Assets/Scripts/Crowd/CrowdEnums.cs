﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdEnums
{
    public enum BehaviourType
    {
        None = -1,      // Crowd member is not going to jump
        Normal,         // We are just jumping randomly
        Influencer,
        NewWave,
        JumpTogether,
        Mosh,
        SurgeForward,
        Count
    }
}
