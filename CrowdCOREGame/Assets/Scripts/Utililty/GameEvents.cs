using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CCEvents
{
    [Serializable]
    public class SurferHitDeadZone : UnityEvent
    {

    }

    [Serializable]
    public class PlayerJoined : UnityEvent<SurferPlayer>
    {

    }
}