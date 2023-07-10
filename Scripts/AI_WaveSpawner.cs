using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_WaveSpawner : MonoBehaviour
{
    bool PlayerInArea = false;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.GetComponent<mouse>() != null)
        {
            PlayerInArea = true;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.GetComponent<mouse>() != null)
        {
            PlayerInArea = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.GetComponent<mouse>() != null)
        {
            PlayerInArea = false;
        }
    }

    public bool isPlayerInArea()
    {
        return PlayerInArea;
    }
}//EndScript