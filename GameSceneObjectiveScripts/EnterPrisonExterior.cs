using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterPrisonExterior : MonoBehaviour
{
    public robotAIScript[] securityBots;
    
    void Start()
    {
        foreach (robotAIScript bot in securityBots)
        {
            bot.PlayerWearingDisguise();
        }
    }

    public void ExitPrison()
    {
        GameObject.Find("playerBody").GetComponent<GoToNextScene>().GoingToNextLevel();
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex + 1);
    }
}//EndScript