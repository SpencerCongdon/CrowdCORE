using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMember : MonoBehaviour
{
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
    private float mMaxRandHorizontalFactor = 0.1f;

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

        if (!mIsJumpDelayed)
        {
            if (mCurrentInfluence == CrowdEnums.InfluenceType.Normal ||
                mCurrentInfluence == CrowdEnums.InfluenceType.Influencer)
            {
                if (IsGrounded())
                {
                    // If you are the influencer we want you to be consistent.
                    if (mCurrentInfluence != CrowdEnums.InfluenceType.Influencer)
                    {
                        mCurrentJumpForce = Random.Range(mMinJumpForce, mMaxJumpForce);
                    }

                    DoJump();
                }
            }
            else if (mCurrentInfluence == CrowdEnums.InfluenceType.JumpTogether)
            {
                if (IsGrounded() && mCurrentInfluencer.IsGrounded())
                {
                    mCurrentJumpForce = mCurrentInfluencer.CurrentJumpForce;
                    DoJump();
                }
            }
            else if (mCurrentInfluence == CrowdEnums.InfluenceType.WaveJump)
            {
                if (IsGrounded() && mCurrentInfluencer.IsFalling())
                {
                    mCurrentJumpForce = mCurrentInfluencer.CurrentJumpForce;
                    DoJump();
                }
            }
            else if (mCurrentInfluence == CrowdEnums.InfluenceType.Mosh)
            {
                Vector3 dirToTarget = mCurrentInfluencer.transform.position - transform.position;
                Vector3 dirNorm = dirToTarget.normalized;
                if(mMoshTimer <= 0.0f)
                {
                    mMoshTimer = mMoshTimeDelay;
                    mRb.AddForce(dirNorm * mMoshForce);
                }
                mMoshTimer -= Time.fixedDeltaTime;
            }
            else if (mCurrentInfluence == CrowdEnums.InfluenceType.SurgeForward)
            {
                if (mSurgeTimer <= 0.0f)
                {
                    mSurgeTimer = mSurgeTimeDelay;
                    Vector3 surgeForce = new Vector3(0.0f, mSurgeJumpForce, -1.0f * mSurgeForce);
                    mRb.AddForce(surgeForce);
                }
                mSurgeTimer -= Time.fixedDeltaTime;
            }
        }
    }

    public void UpdateJumpState()
    {
        if (mCurrentJumpState == JumpState.GoingUp)
        {
            if (mRb.velocity.y < 0f)
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


    public void DoJump()
    {
        if (mRb.velocity.y == 0.0f)
        {
            float randHoriZ = Random.Range(-mMaxRandHorizontalFactor, mMaxRandHorizontalFactor);
            float randHoriX = Random.Range(-mMaxRandHorizontalFactor, mMaxRandHorizontalFactor);

            Vector3 forceVect = new Vector3(randHoriX * mCurrentJumpForce, mCurrentJumpForce, randHoriZ * mCurrentJumpForce);
            mRb.AddForce(forceVect);

            mCurrentJumpState = JumpState.GoingUp;
        }

        if (mHasJumpDelay)
        {
            StartCoroutine(WaveJumpDelay());
        }
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
}
