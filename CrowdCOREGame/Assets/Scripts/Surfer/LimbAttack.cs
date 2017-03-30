using UnityEngine;

public class LimbAttack : MonoBehaviour
{
    [SerializeField]
    Surfer surfer;
    public float power;
    public float minLimbVelocity;

    private Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        Debug.Assert(body != null, "Limb attack is on an object without a rigidbody");
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.transform.tag;
        if (collision.transform.CompareTag("Player"))
        {
            Surfer otherSurfer = collision.gameObject.GetComponent<Surfer>();

            if(otherSurfer != null)
            {
                GameLog.LogFormat("Player {0} just hit Player {1}", GameLog.Category.Surfer, surfer.SurferId, otherSurfer.SurferId);

                if (body.velocity.magnitude > minLimbVelocity && surfer.SurferId != otherSurfer.SurferId)
                {
                    Vector3 direction = transform.position - collision.transform.position;
                    direction.y = -direction.y;
                    collision.rigidbody.AddForce(direction.normalized * power, ForceMode.Impulse);
                }
            }
            else
            {
                GameLog.LogError("Hit player doesn't have a Surfer component " + collision.gameObject.name, GameLog.Category.Surfer);
            }
            
        }
    }
}
