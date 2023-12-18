using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bookColor : MonoBehaviour
{
    [SerializeField] Material[] bookMats;
    
    void Start()
    {
        GetComponent<Renderer>().material = bookMats[Random.Range(0, bookMats.Length)];
    }
}//EndScript