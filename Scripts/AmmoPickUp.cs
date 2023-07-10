using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    GameObject Player, DropObj;

    void Start()
    {
        if (GameObject.Find("PlayerCam") != null)
        {
            Player = GameObject.Find("PlayerCam");
            DropObj = transform.parent.gameObject;
        } else
            {
                Invoke("Late_Start", 0.35f);
            }
    }

    void Late_Start()
    {
        Player = GameObject.Find("PlayerCam");
        DropObj = transform.parent.gameObject;
    }

    void Update()
    {
        if (Player == null)
        {
            Late_Start();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.gameObject == Player)
        {
            GameObject.Find("pickUpSound").GetComponent<AudioSource>().Play();
            CollectAmmo();
        }
    }

    void CollectAmmo()
    {
        GameObject.Find("pickUpSound").GetComponent<AudioSource>().Play();
        
        Player.transform.GetComponent<Weapons>().AddAmmo(Random.Range(6, 10), Random.Range(6, 10));
        Destroy(DropObj);
    }
}//EndScript