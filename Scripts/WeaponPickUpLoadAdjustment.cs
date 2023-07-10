using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUpLoadAdjustment : MonoBehaviour
{
    public GameObject Pistol, Rifle; // Scene 9

    void Start()
    {
        Invoke("LevelCorrection", 1);
    }

    void LevelCorrection()
    {
        if (PlayerPrefs.GetInt("HasPistol") == 1)
        {
            Destroy(Pistol);
        }

        if (PlayerPrefs.GetInt("HasRifle") == 1)
        {
            Destroy(Rifle);
        }

        Destroy(this);
    }
}//EndScript