using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrowdCORE
{
    public class UIPromptButton : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Image mImage;

        [SerializeField]
        private UnityEngine.UI.Text mText;

        public void SetData(Sprite sprite, string text)
        {
            mImage.sprite = sprite;
            mText.text = text;
        }
    }
}