using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbAttack : MonoBehaviour
{
    [SerializeField] PlayerStats playerStats;
    public float power;
    public float minLimbVelocity;

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.transform.tag;
        if (collision.transform.CompareTag("handsChecker"))
        {
            Transform parent = collision.transform.parent;

            while(parent.parent != null)
            {
                parent = parent.parent;
            }

            PlayerStats otherPlayerStats = parent.GetComponent<PlayerStats>();
            Rigidbody currentBody = GetComponent<Rigidbody>();

            if(currentBody.velocity.magnitude > minLimbVelocity && otherPlayerStats != null && playerStats.PlayerID != otherPlayerStats.PlayerID)
            {
                Vector3 direction = transform.position - collision.transform.position;
                direction.y = -direction.y;
                collision.rigidbody.AddForce(direction.normalized * power, ForceMode.Impulse);
            }
        }
    }
}
