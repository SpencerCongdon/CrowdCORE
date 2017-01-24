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
            CrowdEnums.InfluenceType randInfluence = CrowdEnums.InfluenceType.None;
            randInfluence = GetRandomWeightedInfluence();

            int randNumInfluencers = Random.Range(1, mMaxInfluencers);

            List<int> excludeInfluences = new List<int>();
            if (randInfluence == CrowdEnums.InfluenceType.SurgeForward)
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
                    crowdie = mCrowdMembers[crowdieIndex];

                    uniqueCrowdie = !excludeInfluences.Contains(crowdieIndex);
                }

                int numCrowdiesInfluenced = Random.Range(0, mCrowdMembers.Count);

                if (randInfluence == CrowdEnums.InfluenceType.SurgeForward)
                {
                    numCrowdiesInfluenced = mCrowdMembers.Count;
                }

                if (crowdie != null && crowdieIndex > -1)
                {
                    excludeInfluences.Add(crowdieIndex);
                    crowdie.StartInfluence(randInfluence, crowdie, numCrowdiesInfluenced);
                }
            }
        }
    }

    private CrowdEnums.InfluenceType GetRandomWeightedInfluence()
    {
        CrowdEnums.InfluenceType rand = CrowdEnums.InfluenceType.None;
        float randVal = Random.Range(0.0f, 1.0f);
        if(randVal <= 0.05f)
        {
            rand = CrowdEnums.InfluenceType.SurgeForward;
        }
        else if (randVal <= 0.50f)
        {
            rand = CrowdEnums.InfluenceType.Mosh;
        }
        else if (randVal <= 1.0f)
        {
            rand = CrowdEnums.InfluenceType.JumpTogether;            
        }
        return rand;
    }

    public void AddCrowdie(CrowdMember crowdie)
    {
        mCrowdMembers.Add(crowdie);
    }
}
