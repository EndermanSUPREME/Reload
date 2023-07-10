using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadeScript : MonoBehaviour
{
    [SerializeField] GameObject explosionVFX;
    [SerializeField] float explosionForce, explosionRadius;
    [SerializeField] LayerMask playerLayer, RobotLayer;
    Rigidbody rb;

    bool globalAlert = false;
    LevelWideAlertness BotNetAlert;

    [SerializeField] GameObject explosionSound;

    // Start is called before the first frame update
    void Start()
    {
        globalAlert = false;

        GameObject botnetScripObj = GameObject.Find("BotNetAlert");
        BotNetAlert = botnetScripObj.GetComponent<LevelWideAlertness>();

        rb = GetComponent<Rigidbody>();
        StartCoroutine(CollisionDelay());
        StartCoroutine(Fuze());
    }

    IEnumerator Fuze()
    {
        yield return new WaitForSeconds(2.5f);
        GrenadeExplode();
    }

    IEnumerator CollisionDelay()
    {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Collider>().enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        rb.drag = 5;
    }

    void GrenadeExplode()
    {
        Instantiate(explosionVFX, transform.position, Quaternion.identity);

        if (!globalAlert && BotNetAlert.enabled)
        {
            BotNetAlert.AlertAllBots();
            globalAlert = true;
        }

        Collider[] playerNearby = Physics.OverlapSphere(transform.position, explosionRadius, playerLayer);

        for (int i = 0; i < playerNearby.Length; i++)
        {
            playerNearby[i].GetComponent<HealthScript>().ExplosiveDeath();
        }


        Collider[] robotNearby = Physics.OverlapSphere(transform.position, explosionRadius, RobotLayer);

        for (int i = 0; i < robotNearby.Length; i++)
        {
            robotNearby[i].GetComponent<HealthScript>().ExplosiveDeath();
        }

        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.5f, ForceMode.Impulse);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }

}//EndScript