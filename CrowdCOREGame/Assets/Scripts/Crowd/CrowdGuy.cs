using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdGuy : MonoBehaviour
{
    public float cooldown = 2000f;
    public float timer = 0.0f;
    Rigidbody rb;

    public float maxJumpForce = 60f;
    public float minJumpForce = 100f;

    public float maxCooldown = 2f;
    public float minCooldown = 0.5f;

    public float maxPush = 20f;
    public float minPush = 30f;

    public Vector3 jumpDirection = new Vector3(0.0f, 1.0f, 0.0f);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update ()
    {
        timer += Time.deltaTime;
        if(timer > cooldown)
        {
            timer = 0.0f;
            cooldown = Random.Range(minCooldown, maxCooldown);
            float jumpForce = Random.Range(minJumpForce, maxJumpForce);
            rb.AddForce(transform.up * jumpForce);
        }
        
    }

    public void PushAside()
    {
        float randX = Random.Range(minPush, maxPush);
        float randZ = Random.Range(minPush, maxPush);
        rb.AddForce(new Vector3(randX, 0.0f, randZ));
    }

    void OnTriggerExit(Collider other)
    {
        if(other.name == "ExitBox")
        {
            Debug.Log(this.name + " has left the box");
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
