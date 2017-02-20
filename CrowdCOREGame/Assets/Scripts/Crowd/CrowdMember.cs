﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMember : MonoBehaviour
{
    // Constants
    private const string JUMP_FUNCTION = "DoJump";
    private const string RESET_FUNCTION = "ResetBehaviour";

    public enum JumpState
    {
        OnGround, 
        GoingUp,
        ComingDown
    }

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

    
    
    void Awake()
    {
        mCurrentInfluence = CrowdEnums.InfluenceType.None;
        mRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        UpdateJumpState();

        if(mCurrentInfluence == CrowdEnums.InfluenceType.NewWave)
        {
            // If we are still in the wave and aren't waiting to jump, do it
            if (IsGrounded() && !IsInvoking(JUMP_FUNCTION)) DoJump();
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
                mCurrentJumpState = JumpState.OnGround;
            }
        }
    }

    /// <summary>
    /// The main function that performs a new jump based on all current variables
    /// </summary>
    public void DoJump()
    {
        if (mCurrentJumpState == JumpState.OnGround)
        {
            DetermineCurrentJumpForce();
            mRb.AddForce(GenerateMovementForce());

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
                float randHoriZ = Random.Range(-mMaxHorizontalForce, mMaxHorizontalForce);
                float randHoriX = Random.Range(-mMaxHorizontalForce, mMaxHorizontalForce);

                returnForce.x = randHoriX * mCurrentJumpForce;
                returnForce.y = mCurrentJumpForce;
                returnForce.z = randHoriZ * mCurrentJumpForce;
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

    public void JoinWave(float initialDelay, float waveTime, float jumpMin = -1, float jumpMax = -1)
    {
        mJumpDelay = initialDelay;
        mBehaviourTime = waveTime;
        mOverrideMinJump = jumpMin;
        mOverrideMaxJump = jumpMax;

        // All of this should probably go in a common function
        // TODO: We're going to need to make sure that other behaviours don't interfere
        if(mCurrentInfluence == CrowdEnums.InfluenceType.None || mCurrentInfluence == CrowdEnums.InfluenceType.Normal)
        {
            StopAllCoroutines();
            if (IsInvoking(JUMP_FUNCTION)) CancelInvoke(JUMP_FUNCTION);

            mCurrentInfluence = CrowdEnums.InfluenceType.NewWave;

            if(initialDelay > 0)
            {
                Invoke(JUMP_FUNCTION, mJumpDelay);
            }
            else
            {
                DoJump();
            }
        }

        Invoke(RESET_FUNCTION, waveTime);
    }

    private void ResetBehaviour()
    {
        mJumpDelay = -1;
        mBehaviourTime = -1;
        mOverrideMinJump = -1;
        mOverrideMaxJump = -1;
        mCurrentInfluence = CrowdEnums.InfluenceType.None; // TODO: return to normal
    }
}
