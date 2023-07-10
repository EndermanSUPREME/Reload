using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTO_MissileAttack_Obstruction : MonoBehaviour
{
    [SerializeField] CTO_BossScript CTO;
    public bool CTO_InTrigger;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform == CTO.transform)
        {
            CTO_InTrigger = true;
            CTO.MissileShot_Obstructed();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform == CTO.transform)
        {
            CTO_InTrigger = false;
            CTO.MissileShot_Clear();
        }
    }
}//EndScript