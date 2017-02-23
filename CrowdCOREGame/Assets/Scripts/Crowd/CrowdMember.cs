using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMember : MonoBehaviour
{
    // Constants
    private const string TRY_JUMP_FUNCTION = "TryToJump";
    private const string JUMP_FUNCTION = "DoJump";
    private const string RESET_FUNCTION = "ResetBehaviour";

    public enum JumpState
    {
        OnGround, 
        GoingUp,
        ComingDown
    }

    [SerializeField]
    private CrowdEnums.InfluenceType mDefaultInfluence = CrowdEnums.InfluenceType.Normal;

    [SerializeField]
    private CrowdEnums.InfluenceType mCurrentInfluence;
    public CrowdEnums.InfluenceType CurrentInfluence
    {
        get { return mCurrentInfluence; }
        set { mCurrentInfluence = value; }
    }

    [SerializeField]
    private CrowdMember mCurrentInfluencer;

    [SerializeField]
    private JumpState mCurrentJumpState;

    [SerializeField]
    private float mInfluenceRadius = 0.5f; // <! Tunable

    [SerializeField]
    private float mMaxJumpHeight;

    [SerializeField]
    private float mMinJumpHeight;

    // To be set by non-normal jump behaviours
    private float mOverrideMinJump = -1f;
    private float mOverrideMaxJump = -1f;
    private float mJumpDelay = -1f;
    private float mJumpInterval = -1;
    private float mBehaviourTime = -1;      // Length of time to perform the behaviour


    private Rigidbody mRb;

    [SerializeField]
    private float mInfluenceTime = 3.0f;

    [SerializeField]
    private float mWaveJumpDelay = 0.2f;
    private bool mHasJumpDelay = false;
    private bool mIsJumpDelayed = false;

    [SerializeField]
    private float mMoshTimeDelay = 1.0f;
    [SerializeField]
    private float mSurgeTimeDelay = 1.0f;

    [SerializeField]
    private Transform mFoot;

    [SerializeField]
    private float mCurrentJumpVelocity = 0.0f;

    [SerializeField]
    private float mCurrentJumpHeight;
    public float CurrentJumpHeight
    {
        get { return mCurrentJumpHeight; }
    }

    // Moshing
    [SerializeField]
    private float mMoshVelocity = 40000.0f;

    // Surging
    [SerializeField]
    private float mSurgeVelocity = 20000.0f;
    [SerializeField]
    private float mSurgeJumpForce = 11000.0f;

    [SerializeField]
    private float mMaxHorizontalForce = 0.1f;

    private float mMoshTimer = 0.0f;

    private float mSurgeTimer = 0.0f;


    // Crowd boundaries
    // TODO: Would be nice to have a better boundary definition
    Vector3 crowdCenter;
    float lowerBoundX;
    float upperBoundX;
    float lowerBoundZ;
    float upperBoundZ;
    
    void Awake()
    {
        Debug.Assert(mMinJumpHeight <= mMaxJumpHeight, "MinJump is bigger than MaxJump - get your shit together");

        mCurrentInfluence = mDefaultInfluence;
        mRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
    }

    void LateUpdate()
    {

    }

    void FixedUpdate()
    {
        UpdateJumpState();

        if (mCurrentInfluence == CrowdEnums.InfluenceType.NewWave)
        {
            // If we are still in the wave and aren't waiting to jump, do it
            if (IsGrounded() && !IsWaitingToJump()) DoJump();
        }
        else if (!mIsJumpDelayed)
        {
            switch(mCurrentInfluence)
            {
                // Jump if we are on the ground
                case CrowdEnums.InfluenceType.Normal:
                case CrowdEnums.InfluenceType.Influencer:
                    if (IsGrounded())
                    {
                        DoJump();
                    }
                    break;

                // Jump if our influencer will be jumping too
                case CrowdEnums.InfluenceType.JumpTogether:
                    if (IsGrounded() && mCurrentInfluencer.IsGrounded())
                    {
                        DoJump();
                    }
                    break;

                // Jump if our influencer is on his way down
                case CrowdEnums.InfluenceType.WaveJump:
                    if (IsGrounded() && mCurrentInfluencer.IsFalling())
                    {
                        DoJump();
                    }
                    break;

                // TODO: change behaviours that have timers to use the same one
                case CrowdEnums.InfluenceType.Mosh:
                    if (mMoshTimer <= 0.0f)
                    {
                        mMoshTimer = mMoshTimeDelay;
                        DoJump();
                    }
                    mMoshTimer -= Time.fixedDeltaTime;
                    break;
                case CrowdEnums.InfluenceType.SurgeForward:
                    if (mSurgeTimer <= 0.0f)
                    {
                        mSurgeTimer = mSurgeTimeDelay;
                        DoJump();
                    }
                    mSurgeTimer -= Time.fixedDeltaTime;
                    break;
                default:
                    // Do nothing
                    break;
            }
        }
    }

    public void UpdateJumpState()
    {
        if (mCurrentJumpState == JumpState.GoingUp)
        {
            // As soon as they reach the top, they are coming down
            if (mRb.velocity.y <= 0f)
            {
                mCurrentJumpState = JumpState.ComingDown;
            }
        }
        else if (mCurrentJumpState == JumpState.ComingDown)
        {
            // Calculate if we've landed
            if (Physics.Raycast(mFoot.position, -1.0f * mFoot.up, 0.1f))
            {
                Vector3 newVel = mRb.velocity;
                newVel.y = 0;
                mRb.velocity = newVel;
                mCurrentJumpState = JumpState.OnGround;
            }
        }
    }

    /// <summary>
    /// Function that guards handles all logic about whether or not we are allowed to jump
    /// </summary>
    private void TryToJump()
    {
        // If we are waiting to jump, continue to wait
        if (!IsWaitingToJump())
        {
            // If we are grounded jump now
            if (IsGrounded())
            {
                DoJump();
            }
            // If we are still in the air, any only jump on intervals, schedule next jump
            else if (mJumpInterval > 0)
            {
                Invoke(TRY_JUMP_FUNCTION, mJumpInterval);
            }
        }
    }

    /// <summary>
    /// The main function that performs a new jump based on all current variables
    /// </summary>
    /// <remarks>
    /// DO NOT call this directly. Instead, call TryToJump so that we aren't jumping when we shouldn't be
    /// </remarks>
    private void DoJump()
    {
        Debug.Assert(!IsWaitingToJump(), "Should not be calling DoJump while we are waiting!");

        DetermineCurrentJumpVelocity();
        mRb.AddForce(GenerateMovementVelocity(), ForceMode.VelocityChange);
        mCurrentJumpState = JumpState.GoingUp;

        // TODO: I'm not sure the jump function should be determining delays
        if (mHasJumpDelay)
        {
            StartCoroutine(WaveJumpDelay());
        }
    }

    /// <summary>
    /// Determines if we are waiting for our next jump
    /// </summary>
    /// <returns>True if we are waiting</returns>
    private bool IsWaitingToJump()
    {
        return IsInvoking(TRY_JUMP_FUNCTION);
    }

    /// <summary>
    /// Updates mCurrentJumpVelocity using the current crowd member information.
    /// </summary>
    private void DetermineCurrentJumpVelocity()
    {
        switch (mCurrentInfluence)
        {
            // The random jump cases
            case CrowdEnums.InfluenceType.Normal:
            case CrowdEnums.InfluenceType.NewWave:
                float min = mOverrideMinJump >= 0 ? mOverrideMinJump : mMinJumpHeight;
                float max = mOverrideMaxJump >= 0 ? mOverrideMaxJump : mMaxJumpHeight;
                mCurrentJumpHeight = Random.Range(min, max);
                break;

            // The cases where we take the influencer's jump force
            case CrowdEnums.InfluenceType.JumpTogether:
            case CrowdEnums.InfluenceType.WaveJump:
                mCurrentJumpHeight = mCurrentInfluencer.CurrentJumpHeight;
                break;

            // Cases with special values
            case CrowdEnums.InfluenceType.SurgeForward:
                mCurrentJumpHeight = mSurgeJumpForce;
                break;

            // Other special cases where characters may not be jumping
            case CrowdEnums.InfluenceType.Mosh:
            case CrowdEnums.InfluenceType.None:
                mCurrentJumpHeight = 0;
                break;

            default:
                // Otherwise, don't update the jump force
                break;
        }

        mCurrentJumpVelocity = CalculateJumpVelocity(mCurrentJumpHeight);
    }

    /// <summary>
    /// Generate a velocity vector to apply to the crowd member at this moment. All movement logic 
    /// that is NOT jumping, should be performed in here.
    /// </summary>
    /// <returns>The force to be applied</returns>
    private Vector3 GenerateMovementVelocity()
    {
        Vector3 returnVelocity = new Vector3();

        // We currently just switch on the influence type, but there could be other options
        switch(mCurrentInfluence)
        {
            // The cases where we jump straight up and down
            case CrowdEnums.InfluenceType.NewWave:
                returnVelocity.y = mCurrentJumpVelocity;
                break;

            // The cases where crowd members drift around
            case CrowdEnums.InfluenceType.Normal:
            case CrowdEnums.InfluenceType.JumpTogether:
            case CrowdEnums.InfluenceType.WaveJump:
                float xForce = 0;
                float zForce = 0;
                // Should this be a separate behaviour?
                if(!IsInBounds())
                {
                    // Move back into the spawn area
                    Vector3 toCenter = (crowdCenter - this.transform.position).normalized;
                    xForce = toCenter.x * mMaxHorizontalForce;
                    zForce = toCenter.z * mMaxHorizontalForce;
                }
                else
                {
                    xForce = Random.Range(-mMaxHorizontalForce, mMaxHorizontalForce);
                    zForce = Random.Range(-mMaxHorizontalForce, mMaxHorizontalForce);
                }

                returnVelocity.x = xForce;
                returnVelocity.y = mCurrentJumpVelocity;
                returnVelocity.z = zForce;
                break;

            // While moshing we move towards a chosen point
            case CrowdEnums.InfluenceType.Mosh:
                Vector3 dirToTarget = mCurrentInfluencer.transform.position - transform.position;
                dirToTarget.Normalize();
                returnVelocity = dirToTarget * mMoshVelocity;
                break;

            // While surging we move together in one direction
            case CrowdEnums.InfluenceType.SurgeForward:
                // TODO: We should be surging in a specific direction, not just along the Z axis
                returnVelocity = new Vector3(0.0f, mCurrentJumpVelocity, -1.0f * mSurgeVelocity);
                break;

            // For cases where we don't have any non-jump modifiers
            case CrowdEnums.InfluenceType.None:
            default:
                // We don't modify the empty vector
                break;
        }

        return returnVelocity;
    }

    public bool IsGrounded()
    {
        return mCurrentJumpState == JumpState.OnGround;
    }

    public bool IsFalling()
    {
        return mCurrentJumpState == JumpState.ComingDown;
    }

    /// <summary>
    /// Check if this crowd member is within the area it was spawned.
    /// </summary>
    /// <returns>True if inside spawn area</returns>
    public bool IsInBounds()
    {
        bool inX = transform.position.x <= upperBoundX && transform.position.x >= lowerBoundX;
        bool inZ = transform.position.z <= upperBoundZ && transform.position.z >= lowerBoundZ;
        return inX && inZ;
    }

    /// <summary>
    /// Set the boundaries of the crowd spawn area this member is a part of.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="lowerX"></param>
    /// <param name="upperX"></param>
    /// <param name="lowerZ"></param>
    /// <param name="upperZ"></param>
    public void SetCrowdBounds(Vector3 center, float lowerX, float upperX, float lowerZ, float upperZ)
    {
        crowdCenter = center;
        lowerBoundX = lowerX;
        upperBoundX = upperX;
        lowerBoundZ = lowerZ;
        upperBoundZ = upperZ;
    }

    private IEnumerator ClearInfluence()
    {
        yield return new WaitForSeconds(mInfluenceTime);
        mCurrentInfluencer = null;
        mCurrentInfluence = CrowdEnums.InfluenceType.Normal;
        mHasJumpDelay = false;
    }

    private IEnumerator WaveJumpDelay()
    {
        mIsJumpDelayed = true;
        yield return new WaitForSeconds(mWaveJumpDelay);
        mIsJumpDelayed = false;
    }

    public void StartInfluence(CrowdEnums.InfluenceType influenceType, CrowdMember influencer, int count)
    {
        CrowdInfluenceData data = new CrowdInfluenceData(influenceType, influencer, count);
        mCurrentInfluence = CrowdEnums.InfluenceType.Influencer;
        if(mCurrentInfluence == CrowdEnums.InfluenceType.WaveJump)
        {
            mHasJumpDelay = true;
        }
        ApplyInfluence(data);
        StartCoroutine(ClearInfluence());
    }
    // Applys an influence to surrounding crowdmembers.
    public void ApplyInfluence(CrowdInfluenceData data)
    {
        LayerMask mask = 1 << LayerMask.NameToLayer("CrowdMember");
        Collider[] crowdMembers = Physics.OverlapSphere(this.transform.position, mInfluenceRadius, mask);
        if(data.Count > 0)
        {
            for (int i = 0; i < crowdMembers.Length; i++)
            {
                CrowdMember crowdie = crowdMembers[i].GetComponent<CrowdMember>();
                if (crowdie != this)
                {
                    if (crowdie != null)
                    {
                        // Only influence those who are not influenced.
                        if (crowdie.CurrentInfluence == CrowdEnums.InfluenceType.Normal)
                        {
                            data.Count--;
                            crowdie.OnInfluenced(data);
                        }
                    }
                }
            }
        }
    }

    public void OnInfluenced(CrowdInfluenceData data)
    {
        if (mCurrentInfluence == CrowdEnums.InfluenceType.Normal)
        {   
            // Store the influence
            mCurrentInfluencer = data.Influencer;
            mCurrentInfluence = data.Influence;

            switch (data.Influence)
            {
                case CrowdEnums.InfluenceType.JumpTogether:
                    ApplyInfluence(data);
                    break;
                case CrowdEnums.InfluenceType.WaveJump:
                    mHasJumpDelay = true;
                    data.Influencer = this;
                    ApplyInfluence(data);
                    break;
                case CrowdEnums.InfluenceType.Mosh:
                    mMoshTimer = 0.0f;
                    break;
                case CrowdEnums.InfluenceType.SurgeForward:
                    ApplyInfluence(data);
                    break;
                default:
                    break;
            }

            StartCoroutine(ClearInfluence());
        }
    }

    /// <summary>
    /// Alters the crowd member's behaviour to conform with a crowd wave.
    /// </summary>
    /// <param name="initialDelay">Time the member is from the origin of the wave</param>
    /// <param name="waveTime">Duration of the wave behaviour</param>
    /// <param name="jumpMin">Minimum jump height</param>
    /// <param name="jumpMax">Maximum jump height</param>
    /// <param name="imposeDelay">Forces the member to wait for the wave to get to them before jumping</param>
    public void JoinWave(float initialDelay, float waveTime, float jumpMax, float jumpMin = -1, bool imposeDelay = false)
    {
        // Erase all old behaviour stuff
        ResetBehaviour();

        // Set new values
        mJumpDelay = initialDelay;
        mBehaviourTime = waveTime;
        mOverrideMaxJump = jumpMax;

        // If we weren't provided with a minimum, all jumps are the same height
        mOverrideMinJump = jumpMin != -1 ? jumpMin : jumpMax;

        // The member could jump anywhere within the given range, shoot for the average for the interval
        float averageJump = (mOverrideMaxJump + mOverrideMinJump) * .5f;
        float velocity = CalculateJumpVelocity(averageJump);
        mJumpInterval = CalculateJumpDuration(velocity);

        if(!imposeDelay)
        {
            // If we don't want everyone to wait for the wave, we can have them jump in time with the wave
            // by dividing the delay by the interval, so we get their immediate start time
            initialDelay = initialDelay % mJumpInterval;
        }

        // Everything is ready, but before we try to jump, set the active behaviour
        mCurrentInfluence = CrowdEnums.InfluenceType.NewWave;

        if (initialDelay > 0)
        {
            Invoke(TRY_JUMP_FUNCTION, initialDelay);
        }
        else
        {
            TryToJump();
        }

        // Make sure to end the behaviour after the provided period of time
        Invoke(RESET_FUNCTION, waveTime);
    }

    /// <summary>
    /// Reset all information having to do with any currently active behaviours.
    /// </summary>
    private void ResetBehaviour()
    {
        StopAllCoroutines();
        if (IsInvoking(TRY_JUMP_FUNCTION)) CancelInvoke(TRY_JUMP_FUNCTION);

        mJumpDelay = -1;
        mJumpInterval = -1;
        mBehaviourTime = -1;
        mOverrideMinJump = -1;
        mOverrideMaxJump = -1;
        mCurrentInfluence = mDefaultInfluence;
    }

    /// <summary>
    /// Calculate the necessary initial velocity required to jump to a certain height
    /// </summary>
    /// <param name="desiredHeight">The heigh we want to achieve</param>
    /// <returns>Initial veloctiy required for jump</returns>
    private float CalculateJumpVelocity(float desiredHeight)
    {
        float g = Physics.gravity.magnitude;
        float initialV = Mathf.Sqrt(desiredHeight * 2 * g);
        return initialV;
    }
    
    /// <summary>
    /// Time to complete a jump given an initial velocity
    /// </summary>
    /// <param name="initialVelocity">The initial vertical velocity</param>
    /// <returns>Jump duration in seconds</returns>
    private float CalculateJumpDuration(float initialVelocity)
    {
        return 2 * initialVelocity / Physics.gravity.magnitude;
    }
}
