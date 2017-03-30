using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILaserMovement {

    Vector3 CalculatePosition(Vector3 pointBase, Vector3 otherBase, float time);
}
