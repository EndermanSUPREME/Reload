using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonGoToNextDay : MonoBehaviour
{
    public Pod_BotScript podGaurd;
    
    public void SleepInCell()
    {
        if (!GameObject.Find("PlayerCellTrigger").GetComponent<PrisonSchedule>().GetTimeOfDay())
        {
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn_Event();
            GameObject.Find("playerBody").GetComponent<movement>().enabled = false;
            GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ResetPrisonObjective();
            podGaurd.ResetBot();
            Invoke("WakeUpInCell", 3);
        }
    }

    void WakeUpInCell()
    {
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeOut();
        GameObject.Find("PlayerCellTrigger").GetComponent<PrisonSchedule>().BeginNewPodDay();
        GameObject.Find("playerBody").GetComponent<movement>().enabled = true;
    }
}//EndScript