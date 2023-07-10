using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SabatageFuelPumps : MonoBehaviour
{
    bool PumpOne = false, PumpTwo = false;
    public Renderer elevatorButtonRend;
    public Material Red, Green;

    void Start()
    {
        PlayerPrefs.SetInt("InCTO_Fight", 0);

        if (GameObject.Find("ScreenFade") != null)
        {
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeOut();
        }
    }

    public void DisablePumpOne()
    {   
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        PumpOne = true;
    }

    public void DisablePumpTwo()
    {
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        PumpTwo = true;
    }

    void Update()
    {
        if (PumpOne && PumpTwo)
        {
            GameObject botnetScripObj = GameObject.Find("BotNetAlert");
            LevelWideAlertness BotNetAlert = botnetScripObj.GetComponent<LevelWideAlertness>();

            BotNetAlert.DisableForceStealth();

            elevatorButtonRend.material = Green;
        } else
            {
                elevatorButtonRend.material = Red;
            }
    }

    public void GoToUpperLevel()
    {
        if (PumpOne && PumpTwo)
        {
            GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
            GameObject.Find("playerBody").GetComponent<GoToNextScene>().GoingToNextLevel();
            
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().LoadWithCurrentPlayer();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}//EndScript