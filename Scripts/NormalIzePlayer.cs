using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalIzePlayer : MonoBehaviour
{
    public Material bodyPartMat;
    bool changePlayerColor = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("PlayerCam") != null)
        {
            Transform playerCamera = GameObject.Find("PlayerCam").transform;
            playerCamera.gameObject.layer = 7; // regular player layer
    
            if (bodyPartMat != null)
            {
                ResetPlayerAppearence();
            }
        } else
            {
                Invoke("DelayNormalize", 1);
            }
    }

    void Update()
    {
        if (changePlayerColor)
        {
            GameObject[] bodyParts = GameObject.FindGameObjectsWithTag("bodyParts");

            if (bodyPartMat != null && bodyParts != null)
            {
                foreach (GameObject part in bodyParts)
                {
                    part.transform.GetComponent<Renderer>().material = bodyPartMat;
                }
            }

            if (bodyParts != null && bodyParts.Length > 0)
            {
                if (bodyParts[2].transform.GetComponent<Renderer>().material == bodyPartMat)
                {
                    changePlayerColor = false;
                }
            }
        }
    }

    void DelayNormalize()
    {
        Transform playerCamera = GameObject.Find("PlayerCam").transform;
        playerCamera.gameObject.layer = 7; // regular player layer

        if (bodyPartMat != null)
        {
            ResetPlayerAppearence();
        }
    }
    
    void ResetPlayerAppearence()
    {
        changePlayerColor = true;
    }

}//EndScript