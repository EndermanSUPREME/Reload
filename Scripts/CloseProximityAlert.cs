using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseProximityAlert : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] robotAIScript botScript;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform == Player)
        {
            // Alert
            botScript.AttackedByPlayer();
        }
    }
}//EndScript