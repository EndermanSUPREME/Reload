using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionBarRemover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("DetectionBars") != null)
        {
            Destroy(GameObject.Find("DetectionBars"));
        } else
            {
                Invoke("RemoveBars", 0.25f);
            }
    }

    void RemoveBars()
    {
        Destroy(GameObject.Find("DetectionBars"));
    }
}//EndScript