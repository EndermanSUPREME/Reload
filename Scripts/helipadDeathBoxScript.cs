using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class helipadDeathBoxScript : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform == GameObject.Find("PlayerCam").transform)
        {
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeInAndAdjustExternalMusic();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex);
        }
    }
}