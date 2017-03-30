using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A specific type of laser, where the start point always stays rooted at the 
/// transform of it's parent gameobject
/// </summary>
public class AnchoredLaser : Laser
{

    public override Vector3 StartBase { get { return gameObject.transform.position; } }

}
