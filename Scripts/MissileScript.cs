using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    Vector3 pointOfImpact;
    public int DamageInt, explosionRadius;
    public GameObject explosionVFX;

    void Start()
    {
        Invoke("EnableCollisions", 2.25f);
    }

    void EnableCollisions()
    {
        GetComponent<Collider>().enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        pointOfImpact = collision.contacts[0].point;

        Explode(pointOfImpact);
    }

    public void BulletHitRocket(Vector3 point)
    {
        Explode(point);
    }

    void Explode(Vector3 point)
    {
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, point, Quaternion.identity);
        }

        Collider[] NearbyBodies = Physics.OverlapSphere(point, explosionRadius);

        for (int i = 0; i < NearbyBodies.Length; i++)
        {
            if (NearbyBodies[i].GetComponent<HealthScript>() != null)
            {
                NearbyBodies[i].GetComponent<HealthScript>().TakeDamage(DamageInt);
            }
        }

        Destroy(gameObject);
    }
}//EndScript