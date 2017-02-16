using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrowdCORE
{
    /// <summary>
    /// All of our UI Shit should be done in here.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        // Constants
        private static readonly string Identifier = "CrowdCore.UIManager";

        // Serialized Fields
        [SerializeField]
        private Animator mAnimator;

        [SerializeField]
        private Canvas mCanvas;

        [SerializeField]
        private List<UIScreenPrefabInfo> mPrefabInfo;

        // State Variables
        private UIBackgroundState mCurrent3dBackgroundState = UIBackgroundState.None;
        private UIScreenId mCurrentScreenId = UIScreenId.None;
        private UIScreenId mPreviousScreenId = UIScreenId.None;
        private UIBaseScreen mCurrentScreen;

        // Animation Locks
        private bool m3dAnimLock = false;
        private bool m2dAnimLock = false;
        public bool IsAnimLocked
        {
            get
            {
                return m3dAnimLock || m2dAnimLock;
            }
        }
        
        // Input Locks
        private bool mInputLock = false;
        public bool IsInputLocked { get { return mInputLock; } }

        // Loading Locks
        private bool mPrefabLoadingLock = false;

        public override void Awake()
        {
            Debug.AssertFormat(ValidateManager() != false, "{0} : Failed to validate, please ensure that all required components are set and not null.", UIManager.Identifier);
            base.Awake();
        }

        public void Start()
        {
            // This is not something we want to keep in the start.
            TransitionToScreen(UIScreenId.Splash);
        }

        public void Update()
        {
            // TODO: This is super hacky for now.
            if(!mInputLock && mCurrentScreen != null && mCurrentScreen.CanNavigateBack)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Rewired.ReInput.players.GetPlayer(i).GetButtonDown(RewiredConsts.ACTION.Back))
                    {
                        TransitionToScreen(mPreviousScreenId);
                    }
                }
            }
        }

        // Validate all required SerializedFields
        private bool ValidateManager()
        {
            bool isValid = false;

            isValid = (mAnimator != null);

            isValid = isValid && (mCanvas != null);

            isValid = isValid && ValidateScreenList();

            return isValid;
        }

        // Validate the screen list.
        private bool ValidateScreenList()
        {
            bool isValid = false;

            isValid = (mPrefabInfo != null) && (mPrefabInfo.Count > 0);

            List<UIScreenId> tempIds = new List<UIScreenId>();

            if (isValid)
            {
                for (int i = 0; i < mPrefabInfo.Count; i++)
                {
                    if (!tempIds.Contains(mPrefabInfo[i].ScreenId))
                    {
                        tempIds.Add(mPrefabInfo[i].ScreenId);
                    }
                    else
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Set the 2d Animation Lock. (if needed).
        /// </summary>
        /// <param name="locked"></param>
        public void Set2DAnimLock(bool locked)
        {
            m2dAnimLock = locked;
        }

        /// <summary>
        /// Set the 3dAnimation Lock (if needed)
        /// </summary>
        /// <param name="locked"></param>
        public void Set3DAnimLock(bool locked)
        {
            m3dAnimLock = locked;
        }

        /// <summary>
        /// Set the prefab loading lock
        /// </summary>
        /// <param name="locked"></param>
        public void SetPrefabLoadingLock(bool locked)
        {
            mPrefabLoadingLock = locked;
        }

        /// <summary>
        /// Perform a 3d Scene Camera Transition
        /// </summary>
        /// <param name="state"></param>
        public void Transition3dCamera(UIBackgroundState state)
        {
            if(state != UIBackgroundState.None)
            {
                if (state != mCurrent3dBackgroundState)
                {
                    if (mAnimator != null)
                    {
                        // Set the lock. 
                        m3dAnimLock = true;
                        mAnimator.SetTrigger(GetAnimatorParamFromState(state));
                    }
                }

                mCurrent3dBackgroundState = state;
                // Early out so m3dAnimLock will get set by the animation.
                return;
            }

            m3dAnimLock = false;
        }

        /// <summary>
        /// Get the animator parameter from the 3d scene state. We might have many of these.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private string GetAnimatorParamFromState(UIBackgroundState state)
        {
            // TODO: Fix this when we have more final animations
            return "Switch";
            string param = "";
            switch (state)
            {
                case UIBackgroundState.None:
                    param = "";
                    break;
                default:
                    Debug.AssertFormat(true, "{0} : The state {1} is not a valid state for an animation transition.", UIManager.Identifier, state.ToString());
                    break;
            }

            return param;
        }

        /// <summary>
        /// Handle UI Navigation Input 
        /// </summary>
        /// <param name="buttonInfo"></param>
        public void OnUIInput(UIButtonInfo buttonInfo)
        {
            if(!mInputLock)
            {
                switch (buttonInfo.NavType)
                {
                    case UINavigationType.Advance:
                        UIScreenId toScreen = buttonInfo.AdvanceScreen;
                        TransitionToScreen(toScreen);
                        break;
                    case UINavigationType.Back:
                        TransitionToScreen(mPreviousScreenId);
                        break;
                    case UINavigationType.None:
                        break;
                    default:
                        break;
                }
            }
   
        }

        /// <summary>
        /// Transition to a screen id.
        /// </summary>
        /// <param name="screenId"></param>
        public void TransitionToScreen(UIScreenId screenId)
        {
            StartCoroutine(DoScreenTransition(screenId));
        }

        /// <summary>
        /// Event handler for 3d animation events (can play music, whatever.)
        /// </summary>
        /// <param name="animEvent"></param>
        public void On3dUIBackgroundAnimEvent(UIBackgroundAnimEvent animEvent)
        {
            switch(animEvent)
            {
                case UIBackgroundAnimEvent.None:
                    break;
                case UIBackgroundAnimEvent.Start:
                    break;
                case UIBackgroundAnimEvent.End:
                    m3dAnimLock = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Event handler for 2d animation events
        /// </summary>
        /// <param name="animEvent"></param>
        public void OnUIScreenAnimEvent(UIScreenAnimEvent animEvent)
        {
            switch (animEvent)
            {
                case UIScreenAnimEvent.None:
                    break;
                case UIScreenAnimEvent.Start:
                    break;
                case UIScreenAnimEvent.End:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Loads a screen based on it's screen id.
        /// </summary>
        /// <param name="screenId">Unique identifier for a screen.</param>
        private void LoadScreen(UIScreenId screenId)
        {
            GameObject screenPrefab = GetPrefabFromScreenId(screenId);

            if(screenPrefab != null)
            {
                // Instantiate the screen in the canvas.
                GameObject instantiatedPrefab = GameObject.Instantiate(screenPrefab, mCanvas.transform);

                if(instantiatedPrefab != null)
                {
                    UIBaseScreen screen = instantiatedPrefab.GetComponent<UIBaseScreen>();

                    if(screen != null)
                    {
                        // Call set defaults and assign the current screen.
                        screen.Initialize();

                        // Update our screen Ids
                        mPreviousScreenId = mCurrentScreenId;
                        mCurrentScreenId = screenId;

                        // Keep a reference of the screen as an object so we can destroy it or reference it's components.
                        mCurrentScreen = screen;
                    }
                }
            }
        }

        /// <summary>
        /// Unloads the current screen.
        /// Will likely do more stuff in the future.
        /// </summary>
        private void UnloadCurrentScreen()
        {
            if(mCurrentScreen != null)
            {
                mCurrentScreen.Shutdown();
                GameObject.Destroy(mCurrentScreen.gameObject);
            }
        }

        /// <summary>
        /// Returns the prefab associated with a screen id.
        /// </summary>
        /// <param name="screenId"></param>
        /// <returns></returns>
        private GameObject GetPrefabFromScreenId(UIScreenId screenId)
        {
            GameObject screenPrefab = null;

            if (screenId != UIScreenId.None)
            {
                UIScreenPrefabInfo info = mPrefabInfo.Find(p => p.ScreenId == screenId);
                screenPrefab = info.Prefab;
                return screenPrefab;
            }

            Debug.AssertFormat(true, "{0} : Couldn't find a prefab for screenid : {1}. Make sure it is serialzied in mPrefabInfo.", UIManager.Identifier, screenId.ToString());
            return screenPrefab;
        }

        /// <summary>
        /// Transition Work Enumerator.
        /// Carries out the screen loading process and locks the system until a screen has been loaded.
        /// </summary>
        /// <param name="screenId"></param>
        /// <returns></returns>
        private IEnumerator DoScreenTransition(UIScreenId screenId)
        {
            // if this is a new screen... (it should always be.)
            if(screenId != mCurrentScreenId && screenId != UIScreenId.None)
            {
                mInputLock = true;
                if (mCurrentScreen != null)
                {
                    yield return StartCoroutine(mCurrentScreen.DoScreenAnimation(UIScreenAnimState.Outro));
                    UnloadCurrentScreen();
                }

                mPrefabLoadingLock = true;
                LoadScreen(screenId);
                while (mPrefabLoadingLock)
                {
                    yield return null;
                }

                // Fire the 3d background animation.
                UIManager.Instance.Transition3dCamera(mCurrentScreen.BackgroundState);

                yield return StartCoroutine(mCurrentScreen.DoScreenAnimation(UIScreenAnimState.Intro));
                mInputLock = false;
            }
        }
    }
}