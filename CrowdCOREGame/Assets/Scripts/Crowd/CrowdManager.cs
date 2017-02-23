using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : Singleton<CrowdManager>
{
    public bool CrowdCreated = false;

    [SerializeField]
    private CrowdSpawner mSpawner;

    private List<CrowdMember> mCrowdMembers;

    private bool mCrowdLogicOn;

    [SerializeField]
    private float mMinLogicTime = 10.0f;

    [SerializeField]
    private float mMaxLogicTIme = 30.0f;

    [SerializeField]
    private int mMinCrowdies = 50;

    [SerializeField]
    private int mMaxInfluencers = 4;

    [SerializeField]
    private int mInfluenceDuration = 5f;

    public override void Awake()
    {
        mCrowdMembers = new List<CrowdMember>();
        mCrowdLogicOn = false;

        base.Awake();
    }

	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(CrowdCreated && !mCrowdLogicOn)
        {
            StartCoroutine(CrowdLogic());
        }
	}

    private IEnumerator CrowdLogic()
    {
        mCrowdLogicOn = true;
        while(true)
        {
            // Generate a random wait time.
            float waitTime = Random.Range(mMinLogicTime, mMaxLogicTIme);
            yield return new WaitForSeconds(waitTime);
            // Make decision
            CrowdEnums.BehaviourType randInfluence = CrowdEnums.BehaviourType.Normal;
            randInfluence = GetRandomWeightedInfluence();

            int randNumInfluencers = Random.Range(1, mMaxInfluencers);

            List<int> excludeInfluences = new List<int>();
            if (randInfluence == CrowdEnums.BehaviourType.SurgeForward)
            {
                randNumInfluencers = 1;
            }
            for (int i = 0; i < randNumInfluencers; i++)
            {
                bool uniqueCrowdie = false;
                CrowdMember crowdie = null;
                int crowdieIndex = -1;
                while (!uniqueCrowdie)
                {
                    crowdieIndex = Random.Range(mMinCrowdies, mCrowdMembers.Count);

                    // TODO: Why do we keep getting exceptions here
                    crowdie = mCrowdMembers[crowdieIndex];

                    uniqueCrowdie = !excludeInfluences.Contains(crowdieIndex);
                }

                int numCrowdiesInfluenced = Random.Range(0, mCrowdMembers.Count);

                if (randInfluence == CrowdEnums.BehaviourType.SurgeForward)
                {
                    numCrowdiesInfluenced = mCrowdMembers.Count;
                }

                if (crowdie != null && crowdieIndex > -1)
                {
                    excludeInfluences.Add(crowdieIndex);
                    CrowdInfluenceData data = new CrowdInfluenceData(randInfluence, crowdie, numCrowdiesInfluenced, mInfluenceDuration);
                    crowdie.StartInfluence(data);
                }
            }
        }
    }

    private CrowdEnums.BehaviourType GetRandomWeightedInfluence()
    {
        CrowdEnums.BehaviourType rand = CrowdEnums.BehaviourType.Normal;
        float randVal = Random.Range(0.0f, 1.0f);
        if(randVal <= 0.05f)
        {
            rand = CrowdEnums.BehaviourType.SurgeForward;
        }
        else if (randVal <= 0.50f)
        {
            rand = CrowdEnums.BehaviourType.Mosh;
        }
        else if (randVal <= 1.0f)
        {
            rand = CrowdEnums.BehaviourType.JumpTogether;
        }
        return rand;
    }

    public void AddCrowdie(CrowdMember crowdie)
    {
        mCrowdMembers.Add(crowdie);
    }
}
