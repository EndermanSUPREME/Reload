using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prison_Knife_RepositionScript : MonoBehaviour
{
    [SerializeField] Transform[] DestinationPoints;
    
    void Start()
    {
        transform.position = DestinationPoints[Random.Range(0, DestinationPoints.Length)].position;
    }
}//EndScript