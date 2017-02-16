using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrowdCORE
{
    public class UIButtonInfo : MonoBehaviour
    {
        [SerializeField]
        private UINavigationType mNavType = UINavigationType.None;
        public UINavigationType NavType { get { return mNavType; } }

        [SerializeField]
        private UIScreenId mAdvanceScreen = UIScreenId.None;
        public UIScreenId AdvanceScreen { get { return mAdvanceScreen; } }

        public void OnClick()
        {
            UIManager.Instance.OnUIInput(this);
        }
    }
}