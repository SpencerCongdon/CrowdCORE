using UnityEngine;

/// <summary>
/// A class for controlling the movement and action of a Surfer
/// </summary>
[RequireComponent(typeof(Surfer))]
public class SurferControl : MonoBehaviour
{
    public enum PlayerState
    {
        Moving,
        Punch,
        Kick
    }

    public enum RollStatus
    {
        FacingUp,
        FacingDown
    }

    // Necessary for operation
    [SerializeField] private Surfer surfer;

    [SerializeField] private Rigidbody mainBody;
    [SerializeField] private Rigidbody rightHand;
    [SerializeField] private Rigidbody leftHand;
    [SerializeField] private Rigidbody rightFoot;
    [SerializeField] private Rigidbody leftFoot;

    // Movement
    [SerializeField] private float movementPower;
    [SerializeField] private float rollPower;
    [SerializeField] private bool isCameraRelative = true;

    private Camera surfCamera;  // Necessary for camera relative movement

    // Striking
    public float strikeChargeTime = 0.5f;
    public float strikePower;

    [SerializeField] private PlayerState currentState;
    [SerializeField] private RollStatus currentRoll;

    // For inspector
    [SerializeField] private Vector3 lastMovement;

    #region Properties
    public Rigidbody MainBody { get { return mainBody; } }
    public PlayerState CurrentState { get { return currentState; } }
    #endregion // Properties

    #region Monobehaviour Functions
    void Start()
    {
        currentState = PlayerState.Moving;

        surfCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        if (surfer == null)
        {
            surfer = GetComponent<Surfer>();
        }
    }

    void Update () 
    {
        float forwardY = mainBody.transform.forward.y;
        if (currentRoll == RollStatus.FacingDown && forwardY >= 0)
        {
            currentRoll = RollStatus.FacingUp;
        }
        else if (currentRoll == RollStatus.FacingUp && forwardY < 0)
        {
            currentRoll = RollStatus.FacingDown;
        }
    }
    #endregion // Monobehaviour Functions

    #region Movement and Actions
    public void PerformMovement(float xMovement, float zMovement)
    {
        Vector3 movement = Vector3.zero;
        if (isCameraRelative && surfCamera != null)
        {
            movement = (surfCamera.transform.right * xMovement) + (surfCamera.transform.forward * zMovement);
            movement.y = 0;
        }
        else
        {
            movement = new Vector3(xMovement, 0, zMovement);
        }

        lastMovement = movement;

        // Move if necessary
        if (movement != Vector3.zero)
        {
            mainBody.AddForce(movement.normalized * movementPower);
        }
    }

    public void PerformRoll()
    {
        mainBody.AddRelativeTorque(new Vector3(0, rollPower, 0));
    }

    public void PerformPunch()
    {
        LimbStrike(leftHand);
        LimbStrike(rightHand);
    }

    public void PerformKick()
    {
        LimbStrike(leftFoot);
        LimbStrike(rightFoot);
    }
    #endregion // Movement and Actions

    #region Private Functions
    private void LimbStrike(Rigidbody limb)
    {
        Vector3 bodyPosition = mainBody.position;
        Vector3 strikeDirection = (limb.position - bodyPosition);

        limb.transform.position += strikeDirection / 2;
        limb.AddForce(-strikeDirection.normalized * strikePower, ForceMode.Impulse);
    }
    #endregion // Private Functions

    #region Debugging
    void OnDrawGizmos()
    {
        float yPos = transform.position.y;
        const float SCALE = 15;

        // Draw movement result
        Gizmos.color = Color.cyan;
        Vector3 fromPoint = mainBody.position;
        fromPoint.y = yPos;
        Vector3 toPoint = new Vector3(fromPoint.x + (lastMovement.x * SCALE), yPos, fromPoint.z + (lastMovement.z * SCALE));
        Gizmos.DrawLine(fromPoint, toPoint);

        if(isCameraRelative && surfCamera != null)
        {
            // Draw camera forward
            Gizmos.color = Color.red;
            toPoint = fromPoint + (surfCamera.transform.forward * SCALE);
            toPoint.y = yPos;
            Gizmos.DrawLine(fromPoint, toPoint);
        }

        const float ROLL_SCALE = 5;
        Gizmos.color = new Color(1f, 0f, 1f); 
        toPoint = mainBody.transform.position + (mainBody.transform.right * ROLL_SCALE);
        Gizmos.DrawLine(mainBody.transform.position, toPoint);
        toPoint = mainBody.transform.position - (mainBody.transform.right * ROLL_SCALE);
        Gizmos.DrawLine(mainBody.transform.position, toPoint);
        toPoint = mainBody.transform.position - (mainBody.transform.up * ROLL_SCALE);
        Gizmos.DrawLine(mainBody.transform.position, toPoint);
    }
    #endregion // Debugging
}
