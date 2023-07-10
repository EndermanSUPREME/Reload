using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneScreenAdjuster : MonoBehaviour
{
    void Update()
    {
        Screen.SetResolution(PlayerPrefs.GetInt("Width"), PlayerPrefs.GetInt("Height"), true);
    }
}//EndScript