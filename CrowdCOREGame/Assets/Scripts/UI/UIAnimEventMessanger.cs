using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrowdCORE
{
    public class UIAnimEventMessanger : MonoBehaviour
    {
        /// <summary>
        /// Event handler for 3d animation events (can play music, whatever.)
        /// </summary>
        /// <param name="animEvent"></param>
        public void On3dUIBackgroundAnimEvent(UIBackgroundAnimEvent animEvent)
        {
            UIManager.Instance.On3dUIBackgroundAnimEvent(animEvent);
        }

        /// <summary>
        /// Event handler for 2d animation events.
        /// </summary>
        /// <param name="animEvent"></param>
        public void OnUIScreenAnimEvent(UIScreenAnimEvent animEvent)
        {
            UIManager.Instance.OnUIScreenAnimEvent(animEvent);
        }
    }
}
