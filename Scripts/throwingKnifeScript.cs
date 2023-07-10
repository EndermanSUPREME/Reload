using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwingKnifeScript : MonoBehaviour
{
    bool knifeHit = false;
    [SerializeField] GameObject DisplayKnife;

    void Start()
    {
        knifeHit = false;
        StartCoroutine(CollisionDelay());
    }

    IEnumerator CollisionDelay()
    {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Collider>().enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<LimbInfo>() != null)
        {
            collision.transform.GetComponent<LimbInfo>().LimbInjured();
        }

        if (!knifeHit)
        {
            GameObject displayKnife = Instantiate(DisplayKnife, collision.contacts[0].point + collision.contacts[0].normal * 0.001f, Quaternion.identity);
            displayKnife.transform.LookAt(collision.contacts[0].point + collision.contacts[0].normal);
            displayKnife.transform.parent = collision.transform;
                
            knifeHit = true;

            Destroy(gameObject);
        }
    }
}//EndScript