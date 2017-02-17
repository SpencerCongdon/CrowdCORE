using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrowdCORE
{
    public class UIPrompts : MonoBehaviour
    {
        [SerializeField]
        private List<UIPromptsInfo> mPromptsInfo;

        [SerializeField]
        private RectTransform mLeftRect;

        [SerializeField]
        private RectTransform mRightRect;


        [SerializeField]
        private GameObject mPromptPrefab;

        private UIPromptsInfo mCurrentPromptsInfo;

        public void SetPromptsPlatform(InputPlatform platform)
        {
            UIPromptsInfo info = mPromptsInfo.Find(x => x.Platform == platform);

            if(info != null)
            {
                mCurrentPromptsInfo = info;
            }
        }

        public void SetLeftPrompts(List<InputButton> buttons)
        {
            // Spawn them in the left rect
            for(int i = 0; i < buttons.Count; i++)
            {
                GameObject instantiatedPrefab = GameObject.Instantiate(mPromptPrefab, mLeftRect);

                if (instantiatedPrefab != null)
                {
                    UIPromptButton promptButton = instantiatedPrefab.GetComponent<UIPromptButton>();
                    promptButton.SetData(mCurrentPromptsInfo.GetButtonInfo(buttons[i]).tex, mCurrentPromptsInfo.GetButtonInfo(buttons[i]).text);
                }
            }
        }

        public void SetRightPrompts(List<InputButton> buttons)
        {
            // Spawn them in the right rect
            for (int i = 0; i < buttons.Count; i++)
            {
                GameObject instantiatedPrefab = GameObject.Instantiate(mPromptPrefab, mRightRect);

                if (instantiatedPrefab != null)
                {
                    UIPromptButton promptButton = instantiatedPrefab.GetComponent<UIPromptButton>();
                    promptButton.SetData(mCurrentPromptsInfo.GetButtonInfo(buttons[i]).tex, mCurrentPromptsInfo.GetButtonInfo(buttons[i]).text);
                }
            }
        }

        public void ClearPrompts()
        {
            if(mLeftRect != null)
            {
                for (int i = 0; i < mLeftRect.transform.childCount; i++)
                {
                    GameObject.Destroy(mLeftRect.transform.GetChild(i).gameObject);
                }
            }

            if(mRightRect != null)
            {
                for (int i = 0; i < mRightRect.transform.childCount; i++)
                {
                    GameObject.Destroy(mRightRect.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}

