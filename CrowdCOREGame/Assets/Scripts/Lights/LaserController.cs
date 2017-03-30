using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for controlling the lasers in a scene.
/// </summary>
/// <remarks>
/// This class should generally be issuing orders to <see cref="LaserGroup"/> objects.
/// There should generally only be one LaserController within a venue scene.
/// </remarks>
public class LaserController : MonoBehaviour
{

    List<LaserGroup> _groups = new List<LaserGroup>();

    // Use this for initialization
    void Start ()
    {

#if UNITY_EDITOR
        // This allows us to simply add groups to the scene while testing.
        // Final versions of a scene should make sure that all groups are specifically added to the controller,
        // as they will not be automatically added in a standalone build
        GameObject[] objects = GameObject.FindGameObjectsWithTag(Tag.LASER_GROUP);
        foreach(GameObject g in objects)
        {
            LaserGroup group = g.GetComponent<LaserGroup>();
            Debug.Assert(group != null, g.name + " does not have a LaserGroup even though it has the tag");

            if(!_groups.Contains(group))
            {
                GameLog.LogWarning("Please add " + group.name + " to LaserController in editor", GameLog.Category.Lighting);
                _groups.Add(group);
            }
        }
#endif

    }
    
    // Update is called once per frame
    void Update ()
    {
        
    }
}
