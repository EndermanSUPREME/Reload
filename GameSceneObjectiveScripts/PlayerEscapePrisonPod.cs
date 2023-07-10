using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEscapePrisonPod : MonoBehaviour
{
    [SerializeField] Transform PodDoor, PodDoorOpenedPos, podGuard;
    [SerializeField] GameObject doorDongle, disguiseObj;
    [SerializeField] bool openPodDoor = false, playerDisguised = false;
    [SerializeField] robotAIScript[] securityBots;
    public movement PlayerMovement;
    public Renderer buttonRend;
    public Material buttonGreen, buttonRed, enemyBodyPartMat;

    void Update()
    {
        if (!podGuard.GetComponent<Pod_BotScript>().enabled)
        {
            GameObject.Find("PrisonAlert").GetComponent<PrisonAlarm>().PodGaurdDown();

            doorDongle.transform.position = podGuard.transform.position + new Vector3(0, 0.75f, 0);
        }

        if (!doorDongle.active)
        {
            disguiseObj.transform.position = doorDongle.transform.position;
        }

        if (openPodDoor)
        {
            PodDoor.position = Vector3.MoveTowards(PodDoor.position, PodDoorOpenedPos.position, 1 * Time.deltaTime);
        }

        PlayerPrefs.SetInt("HasPistol", 0);
        PlayerPrefs.SetInt("HasRifle", 0);
    }

    public void PlayerDisguise()
    {
        playerDisguised = true;
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn_Event();
        PlayerMovement.enabled = false;

        foreach (robotAIScript bot in securityBots)
        {
            bot.PlayerWearingDisguise();
        }

        Invoke("DisguiseTransition", 2);
        disguiseObj.SetActive(false);
    }

    void DisguiseTransition()
    {
        GameObject[] bodyParts = GameObject.FindGameObjectsWithTag("bodyParts");

        if (enemyBodyPartMat != null)
        {
            foreach (GameObject part in bodyParts)
            {
                part.transform.GetComponent<Renderer>().material = enemyBodyPartMat;
            }
        }
        
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeOut();
        PlayerMovement.enabled = true;
    }

    public bool isPlayerDisguised()
    {
        return playerDisguised;
    }

    public void ScanPrisonDongle()
    {
        if (!doorDongle.active)
        {
            openPodDoor = true;
            GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
            buttonRend.material = buttonGreen;
        } else
            {
                buttonRend.material = buttonRed;
            }
    }
}//EndScript