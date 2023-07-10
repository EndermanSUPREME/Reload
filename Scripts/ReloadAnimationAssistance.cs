using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAnimationAssistance : MonoBehaviour
{
    public Transform pistolMag, rifleMag, magHoldingHand;
    Transform pistol, rifle;
    Quaternion rifleMagRot, pistolMagRot;
    Vector3 rifleMagLoc, pistolMagLoc;

    void Awake()
    {
        pistol = pistolMag.parent;
        rifle = rifleMag.parent;

        rifleMagRot = rifleMag.localRotation;
        pistolMagRot = pistolMag.localRotation;

        rifleMagLoc = rifleMag.localPosition;
        pistolMagLoc = pistolMag.localPosition;
    }

    void TakeRifleMag()
    {
        rifleMag.parent = magHoldingHand;
    }

    void TakePistolMag()
    {
        pistolMag.parent = magHoldingHand;
    }

    public void PlaceRifleMag()
    {
        rifleMag.parent = rifle;

        rifleMag.localRotation = rifleMagRot;
        rifleMag.localPosition = rifleMagLoc;
    }

    public void PlacePistolMag()
    {
        pistolMag.parent = pistol;
        
        pistolMag.localRotation = pistolMagRot;
        pistolMag.localPosition = pistolMagLoc;
    }

}//EndScript