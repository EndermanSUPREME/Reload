using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialEventScript : MonoBehaviour
{
    public bool deathBox = false;

    public void GoToNextLevel()
    {
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadSceneButton()
    {
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (deathBox && collider.transform.GetComponent<playerInteract>() != null)
        {
            ReloadSceneButton();
        }
    }
}//EndScript