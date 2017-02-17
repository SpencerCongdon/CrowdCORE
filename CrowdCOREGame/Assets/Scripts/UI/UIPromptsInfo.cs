using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrowdCORE
{
    [System.Serializable]
    public struct InputButtonInfo
    {
        public Sprite tex;
        public InputButton button;
        public string text;
    }

    [System.Serializable]
    public class UIPromptsInfo
    {
        [SerializeField]
        private InputPlatform mPlatform;
        public InputPlatform Platform { get { return mPlatform; } }

        [SerializeField]
        private List<InputButtonInfo> mButtonPrompts;

        public InputButtonInfo GetButtonInfo(InputButton button)
        {
            InputButtonInfo returnInfo = new InputButtonInfo();

            returnInfo.text = "NO PROMPT";
            returnInfo.button = InputButton.None;
            returnInfo.tex = null;

            if(mButtonPrompts != null && mButtonPrompts.Count > 0)
            {
                returnInfo = mButtonPrompts.Find(x => x.button == button);
            }

            return returnInfo;
        }
    }
}