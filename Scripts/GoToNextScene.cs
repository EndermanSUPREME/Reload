using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextScene : MonoBehaviour
{
    int nextSceneIndex;

    void Start()
    {
        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
    }

    public void TestDontDestroy()
    {
        GoingToNextLevel();
        
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadDueToBadLoad()
    {
        GoingToNextLevel();
        
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == nextSceneIndex)
        {
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeOut();
            Start();
        }
    }

    public void GoingToNextLevel()
    {
        DontDestroyOnLoad(gameObject); // player in prison
    }
}//EndScript