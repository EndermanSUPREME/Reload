using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadLevel_FadeAdjust : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (GameObject.Find("ScreenFade") != null)
        {
            if (GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>() != null)
            {
                GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeOut();
            }
        }
    }
}//EndScript