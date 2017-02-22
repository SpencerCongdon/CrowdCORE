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
    private float mMaxJumpForce = 60f;

    [SerializeField]
    private float mMinJumpForce = 100f;

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
    private float mCurrentJumpForce = 0.0f;
    public float CurrentJumpForce
    {
        get { return mCurrentJumpForce; }
    }

    // Moshing
    [SerializeField]
    private float mMoshForce = 40000.0f;

    // Surging
    [SerializeField]
    private float mSurgeForce = 20000.0f;
    [SerializeField]
    private float mSurgeJumpForce = 11000.0f;

    [SerializeField]
    private float mMaxHorizontalForce = 0.1f;

    private float mMoshTimer = 0.0f;

    private float mSurgeTimer = 0.0f;


    private float jumptime = 0f;
    private float maxHeight = 0f;

    // Crowd boundaries
    Vector3 crowdCenter;
    float lowerBoundX;
    float upperBoundX;
    float lowerBoundZ;
    float upperBoundZ;
    
    void Awake()
    {
        mCurrentInfluence = mDefaultInfluence;
        mRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
    }

    void LateUpdate()
    {

    }

    private bool IsWaitingToJump()
    {
        return IsInvoking(JUMP_FUNCTION) || IsInvoking(TRY_JUMP_FUNCTION);
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
            jumptime += Time.deltaTime;
            // As soon as they reach the top, they are coming down
            if (mRb.velocity.y <= 0f)
            {
                maxHeight = mRb.position.y;
                mCurrentJumpState = JumpState.ComingDown;
            }
        }
        else if (mCurrentJumpState == JumpState.ComingDown)
        {
            jumptime += Time.deltaTime;
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
    /// The main function that performs a new jump based on all current variables
    /// </summary>
    public void DoJump()
    {
        if (IsWaitingToJump()) return;

        if (mCurrentJumpState == JumpState.OnGround)
        {
            DetermineCurrentJumpForce();
            mRb.AddForce(GenerateMovementForce(), ForceMode.VelocityChange);
            jumptime = 0f;
            mCurrentJumpState = JumpState.GoingUp;
        }
        else
        {
            Debug.LogWarning("Should not be calling DoJump when a crowd member isn't on the ground");
        }

        // TODO: I'm not sure the jump function should be determining delays
        if (mHasJumpDelay)
        {
            StartCoroutine(WaveJumpDelay());
        }
    }

    /// <summary>
    /// Updates mCurrentJumpForce using the current crowd member information
    /// </summary>
    private void DetermineCurrentJumpForce()
    {
        switch (mCurrentInfluence)
        {
            // The random jump cases
            case CrowdEnums.InfluenceType.Normal:
            case CrowdEnums.InfluenceType.NewWave:
                float min = mOverrideMinJump >= 0 ? mOverrideMinJump : mMinJumpForce;
                float max = mOverrideMaxJump >= 0 ? mOverrideMaxJump : mMaxJumpForce;
                mCurrentJumpForce = Random.Range(min, max);
                break;

            // The cases where we take the influencer's jump force
            case CrowdEnums.InfluenceType.JumpTogether:
            case CrowdEnums.InfluenceType.WaveJump:
                mCurrentJumpForce = mCurrentInfluencer.CurrentJumpForce;
                break;

            // Cases with special values
            case CrowdEnums.InfluenceType.SurgeForward:
                mCurrentJumpForce = mSurgeJumpForce;
                break;

            // Other special cases where characters may not be jumping
            case CrowdEnums.InfluenceType.Mosh:
            case CrowdEnums.InfluenceType.None:
                mCurrentJumpForce = 0;
                break;

            default:
                // Otherwise, don't update the jump force
                break;
        }
    }

    /// <summary>
    /// Generate a force vector to apply to the crowd member at this moment
    /// </summary>
    /// <returns>The force to be applied</returns>
    private Vector3 GenerateMovementForce()
    {
        Vector3 returnForce = new Vector3();

        // We currently just switch on the influence type, but there could be other options
        switch(mCurrentInfluence)
        {
            // The cases where we jump straight up and down
            case CrowdEnums.InfluenceType.NewWave:
                returnForce.y = CurrentJumpForce;
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
                    Vector3 toCenter = (crowdCenter - this.transform.position).normalized;
                    xForce = toCenter.x * mMaxHorizontalForce;
                    zForce = toCenter.z * mMaxHorizontalForce;
                }
                else
                {
                    xForce = Random.Range(-mMaxHorizontalForce, mMaxHorizontalForce);
                    zForce = Random.Range(-mMaxHorizontalForce, mMaxHorizontalForce);
                }

                returnForce.x = xForce * mCurrentJumpForce;
                returnForce.y = mCurrentJumpForce;
                returnForce.z = zForce * mCurrentJumpForce;
                break;

            // While moshing we move towards a chosen point
            case CrowdEnums.InfluenceType.Mosh:
                Vector3 dirToTarget = mCurrentInfluencer.transform.position - transform.position;
                dirToTarget.Normalize();
                returnForce = dirToTarget * mMoshForce;
                break;

            // While surging we move together in one direction
            case CrowdEnums.InfluenceType.SurgeForward:
                // TODO: We should be surging in a specific direction, not just along the Z axis
                returnForce = new Vector3(0.0f, mCurrentJumpForce, -1.0f * mSurgeForce);
                break;

            // For cases where we don't have any non-jump modifiers
            case CrowdEnums.InfluenceType.None:
            default:
                // We don't modify the empty vector
                break;
        }

        return returnForce;
    }

    public bool IsGrounded()
    {
        return mCurrentJumpState == JumpState.OnGround;
    }

    public bool IsFalling()
    {
        return mCurrentJumpState == JumpState.ComingDown;
    }

    public bool IsInBounds()
    {
        bool inX = transform.position.x <= upperBoundX && transform.position.x >= lowerBoundX;
        bool inZ = transform.position.z <= upperBoundZ && transform.position.z >= lowerBoundZ;
        return inX && inZ;
    }

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

    public void JoinWave(float initialDelay, float waveTime, float waveInterval, float jumpMin = -1, float jumpMax = -1)
    {
        mJumpDelay = initialDelay;
        mBehaviourTime = waveTime;
        mOverrideMinJump = jumpMin;
        mOverrideMaxJump = jumpMax;
        mJumpInterval = waveInterval;

        // All of this should probably go in a common function
        // TODO: We're going to need to make sure that other behaviours don't interfere
        if(mCurrentInfluence == CrowdEnums.InfluenceType.None || mCurrentInfluence == CrowdEnums.InfluenceType.Normal)
        {
            StopAllCoroutines();
            if (IsInvoking(JUMP_FUNCTION)) CancelInvoke(JUMP_FUNCTION);

            mCurrentInfluence = CrowdEnums.InfluenceType.NewWave;

            // initial delay should be offset
            // so when we divide it by the interval, we get their immediate start time

            initialDelay = initialDelay % mJumpInterval;

            if(initialDelay > 0)
            {
                Invoke(TRY_JUMP_FUNCTION, initialDelay);
            }
            else
            {
                TryToJump();
            }
        }

        Invoke(RESET_FUNCTION, waveTime);
    }

    private void ResetBehaviour()
    {
        mJumpDelay = -1;
        mJumpInterval = -1;
        mBehaviourTime = -1;
        mOverrideMinJump = -1;
        mOverrideMaxJump = -1;
        mCurrentInfluence = mDefaultInfluence; // TODO: return to normal
    }

    private void TryToJump()
    {
        if(IsGrounded())
        {
            DoJump();
        }
        else if(mJumpInterval > 0)
        {
            Invoke(TRY_JUMP_FUNCTION, mJumpInterval);
        }
        else
        {
            // I guess we don't get to jump
        }
    }

    //private float VelocityForJump(float desiredHeight)
    //{
    //    float g = Physics.gravity.magnitude;
    //    float initialV = Mathf.Sqrt(desiredHeight * 2 * g);
    //    return initialV;
    //}
    //
    //private float TimeForJump(float initialVelocity)
    //{
    //    return 2 * initialVelocity / Physics.gravity.magnitude;
    //}
}
