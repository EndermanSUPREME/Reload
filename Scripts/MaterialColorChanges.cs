using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColorChanges : MonoBehaviour
{
    [Range(0, 255)] float g, targetG;
    
    public Color start, end, showColor;

    Material mat;

    void Start()
    {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SFXVol");

        g = start.g * 255;

        targetG = end.g * 255;

        mat = GetComponent<Renderer>().material;

        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (g > targetG)
        {
            g-=4;
        }

        showColor = new Color(1, Mathf.Abs(g/255), 0);
        mat.color = showColor;
    }
}//EndScript