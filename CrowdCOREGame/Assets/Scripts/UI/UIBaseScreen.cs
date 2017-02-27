using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using RewiredConsts;

namespace CrowdCORE
{
    public class UIBaseScreen : MonoBehaviour
    {
        [SerializeField]
        private UIScreenId mScreenId = UIScreenId.None;
        public UIScreenId ScreenId { get { return mScreenId; } }

        [SerializeField]
        private UIBackgroundState mBackgroundState = UIBackgroundState.None;
        public UIBackgroundState BackgroundState { get { return mBackgroundState; } }

        [SerializeField]
        private Vector3 mScreenPos = Vector3.zero;

        [SerializeField]
        private Vector3 mScreenScale = Vector3.zero;

        [SerializeField]
        private Quaternion mScreenRotation = Quaternion.identity;

        [SerializeField]
        private Animator mAnimator;

        [SerializeField]
        private bool mCanBack;
        public bool CanNavigateBack { get { return mCanBack; } }

        public virtual void Awake()
        {
            Debug.AssertFormat(ValidateScreen() != false, "Screen with id {0} failed to validate.", mScreenId);
        }

        public virtual void Start()
        {
            UIManager.Instance.SetPrefabLoadingLock(false);
        }

        public virtual void Update()
        {

        }

        protected virtual bool ValidateScreen()
        {
            bool isValid = false;

            isValid = (ScreenId != UIScreenId.None);

            isValid = isValid && (mAnimator != null);

            return isValid;
        }

        public virtual void SetPrompts()
        {
            GameLog.Log("Setting Prompts for: " + name, GameLog.Category.UI);
        }

        public virtual void Initialize()
        {
            transform.localPosition = mScreenPos;
            transform.localScale = mScreenScale;
            transform.localRotation = mScreenRotation;
        }

        public virtual void Shutdown()
        {
            GameLog.Log("Shutting down screen: " + name, GameLog.Category.UI);
        }

        public virtual void On3dUIBackgroundAnimEvent(UIBackgroundAnimEvent animEvent)
        {
            switch (animEvent)
            {
                case UIBackgroundAnimEvent.None:
                    break;
                case UIBackgroundAnimEvent.Start:
                    break;
                case UIBackgroundAnimEvent.End:
                    break;
                default:
                    break;
            }
        }

        public virtual void OnUIScreenAnimEvent(UIScreenAnimEvent animEvent)
        {
            switch (animEvent)
            {
                case UIScreenAnimEvent.None:
                    break;
                case UIScreenAnimEvent.Start:
                    UIManager.Instance.Set2DAnimLock(true);
                    break;
                case UIScreenAnimEvent.End:
                    UIManager.Instance.Set2DAnimLock(false);
                    break;
                default:
                    break;
            }
        }

        public virtual IEnumerator DoScreenAnimation(UIScreenAnimState state)
        {
            // Wait for the animation to finish.
            while(UIManager.Instance.IsAnimLocked)
            {
                yield return null;
            }

            UIManager.Instance.Set2DAnimLock(true);
            PlayScreenAnimation(state);

            while (UIManager.Instance.IsAnimLocked)
            {
                yield return null;
            }
        }

        public virtual void PlayScreenAnimation(UIScreenAnimState state)
        {
            if (mAnimator != null)
            {
                mAnimator.SetTrigger(GetAnimatorParamFromState(state));
            }
        }

        private string GetAnimatorParamFromState(UIScreenAnimState state)
        {
            string param = "";
            switch (state)
            {
                case UIScreenAnimState.None:
                    param = "";
                    break;
                case UIScreenAnimState.Intro:
                    param = "Intro";
                    break;
                case UIScreenAnimState.Outro:
                    param = "Outro";
                    break;
                default:
                    Debug.AssertFormat(true, "{0} : The state {1} is not a valid state for an animation transition.", gameObject.name, state.ToString());
                    break;
            }

            return param;
        }
    }
}