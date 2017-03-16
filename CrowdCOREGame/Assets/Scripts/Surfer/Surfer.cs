using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for tracking base information about the surfer character
/// </summary>
public class Surfer : MonoBehaviour {

    public enum SurferState
    {
        ALIVE = 0,
        DEAD
    }

    [SerializeField]
    private SurferControl control;
    [SerializeField]
    private SurferState currentState = SurferState.ALIVE;
    [SerializeField]
    private SurferLight playerLight;
    [SerializeField]
    public List<Material> playerShirtMaterials;
    [SerializeField]
    public List<SkinnedMeshRenderer> playerShirt;

    public SurferState CurrentState { get { return currentState; } set { currentState = value; } }
    public SurferLight CurrentLight { get { return playerLight; } set { playerLight = value; } }

    [SerializeField]
    protected int surferId = -1;
    public int SurferId { get { return surferId; } }

    // Use this for initialization
    void Start ()
    {
        for (int i = 0; i < playerShirt.Count; i++)
        {
            if (i != 0)
            {
                Material[] currentMats = playerShirt[i].materials;
                currentMats[1] = playerShirtMaterials[0];
                playerShirt[i].materials = currentMats;
            }
            else
            {
                playerShirt[i].material = playerShirtMaterials[0];
            }
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "DeadZone")
        {
            OnHitDeadZone();
        }
    }

    public void OnHitDeadZone()
    {
        if (currentState != Surfer.SurferState.DEAD)
        {
            control.enabled = false;
            currentState = Surfer.SurferState.DEAD;
            playerLight.enabled = false;
            Light light = playerLight.GetComponent<Light>();
            light.enabled = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerDead(SurferId);
            }

            GameLog.Log("Player " + SurferId + " is TOTALLY FUCKING DEAD!!! \\m/ >_< \\m/", GameLog.Category.Surfer);
        }
    }

    // Update is called once per frame
    void Update () {

	}
}
