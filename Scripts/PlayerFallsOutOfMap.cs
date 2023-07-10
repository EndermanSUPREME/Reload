using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFallsOutOfMap : MonoBehaviour
{
    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.name == "PlayerCam")
        {
            GameObject.Find("playerBody").transform.GetComponent<movement>().enabled = false;

            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeInAndAdjustExternalMusic();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex);
        }
    }
}//EndScript