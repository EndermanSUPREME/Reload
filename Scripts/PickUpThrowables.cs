using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpThrowables : MonoBehaviour
{
    GameObject Camera;
    Weapons playerWeaponScript;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] bool explosive;
    [SerializeField] float pickUpRadius;

    void Start()
    {
        Camera = GameObject.Find("PlayerCam");
        playerWeaponScript = Camera.transform.GetComponent<Weapons>();
    }

    void FixedUpdate()
    {
        if (Physics.CheckSphere(transform.position, pickUpRadius, playerLayer))
        {
            if (playerWeaponScript)
            {
                GameObject.Find("pickUpSound").GetComponent<AudioSource>().Play();

                if (!explosive)
                {
                    playerWeaponScript.PickUpKnife();
                } else
                    {
                        playerWeaponScript.PickUpGrenade();
                    }

                Destroy(gameObject);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, pickUpRadius);
    }
}//EndScript