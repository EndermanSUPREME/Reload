using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotBulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Collider>().enabled = false;
        transform.GetComponent<Rigidbody>().velocity = transform.forward * 15;
        Invoke("EnableCollisions", 0.15f);
    }

    void EnableCollisions()
    {
        GetComponent<Collider>().enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<LimbInfo>() != null)
        {
            collision.transform.GetComponent<LimbInfo>().LimbInjured();
        }
        Destroy(gameObject);
    }
}//EndScript