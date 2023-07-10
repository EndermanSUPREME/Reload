using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponsTracker : MonoBehaviour
{
    [SerializeField] GameObject equippedKnife, equippedPistol, equipedRifle;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "WorkPrisonOutside")
        {
            if (transform.name == "newWeaponTracker") // newWeaponsTracker | PlayerWeaponsTracker [newWeaponTracker on the workPrisonOutside scene will copy and store the data from the PlayerWeaponsTracker that the player had during the workPrisonInside scene]
            {
                GameObject K = GameObject.Find("PlayerWeaponsTracker").GetComponent<WeaponsTracker>().GetKnife();
                GameObject P = GameObject.Find("PlayerWeaponsTracker").GetComponent<WeaponsTracker>().GetPistol();
                GameObject R = GameObject.Find("PlayerWeaponsTracker").GetComponent<WeaponsTracker>().GetRifle();

                GameObject.Find("newWeaponTracker").GetComponent<WeaponsTracker>().DataTransfer(K, P, R);

                // print("[+] Pulling Objects From Old Script");
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 11)
        {
            Invoke("Gift_Gear", 1);
        }
    }

    void Gift_Gear()
    {
        if (GameObject.Find("PlayerCam") != null)
        {
            if (SceneManager.GetActiveScene().buildIndex == 11)
            {
                if (equippedPistol == null && equipedRifle == null) // if the player didnt grab any gear before entering the truck
                {
                    GameObject.Find("PlayerCam").GetComponent<Weapons>().AquireKnife();
                    GameObject.Find("PlayerCam").GetComponent<Weapons>().AquireRifle();
                }
            }
        }
    }

//==================================================================//

    public GameObject GetKnife()
    {
        return equippedKnife;
    }

    public GameObject GetPistol()
    {
        return equippedPistol;
    }

    public GameObject GetRifle()
    {
        return equipedRifle;
    }

//==================================================================//

    public void LinkKnife(GameObject obj)
    {
        equippedKnife = obj;
    }

    public void LinkPistol(GameObject obj)
    {
        equippedPistol = obj;
    }

    public void LinkRifle(GameObject obj)
    {
        equipedRifle = obj;
    }

//==================================================================//


    public void DataTransfer(GameObject knf, GameObject pist, GameObject rif)
    {
        equippedKnife = knf;
        equippedPistol = pist;
        equipedRifle = rif;

        GameObject.Find("PlayerCam").GetComponent<Weapons>().LoadKeptWeapons(equippedKnife, equippedPistol, equipedRifle);

        if (transform.name == "newWeaponTracker")
        {
            Destroy(GameObject.Find("PlayerWeaponsTracker"));

            GameObject.Find("newWeaponTracker").transform.parent = GameObject.Find("playerBody").transform;
            transform.name = "PlayerWeaponsTracker";
        }
    }

//==================================================================//


    public void AquireKnifeExternal()
    {
        GameObject.Find("PlayerCam").GetComponent<Weapons>().AquireKnife();
    }

    public void AquirePistolExternal()
    {
        GameObject.Find("PlayerCam").GetComponent<Weapons>().AquirePistol();
    }

    public void AquireRifleExternal()
    {
        GameObject.Find("PlayerCam").GetComponent<Weapons>().AquireRifle();
    }

}//EndScript