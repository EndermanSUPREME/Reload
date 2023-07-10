using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelWideAlertness : MonoBehaviour
{
    [SerializeField] robotAIScript[] botScripts;
    public bool EnemyWaveLevel = false, StrictStealth, trackDeadEnemies = true, inPrison = false;
    bool areaAlerted = false, musicAltered = false, allEnemiesAreDead = false;
    GameObject StrictStealth_UI;
    int deadEnemies;
    public int totalNumOfEnemies;

    void Start()
    {
        if (areaAlerted)
        {
            AlertAllBots();
        }

        if (EnemyWaveLevel)
        {
            trackDeadEnemies = false;
            StartCoroutine(AlertWave());
        }

        Invoke("Display_Strict_Restriction", 0.25f);
    }

    public void EnemyKilled()
    {
        if (trackDeadEnemies)
        {
            deadEnemies++;

            if (deadEnemies == totalNumOfEnemies)
            {
                // all enemies are dead
                EnemiesAreAllDead();
            }
        }
    }

    void EnemiesAreAllDead()
    {
        if (GameObject.Find("PlayerCam") != null && GameObject.Find("PlayerCam").GetComponent<AudioFades>() != null)
        {
            GameObject.Find("PlayerCam").GetComponent<AudioFades>().ChangeMusicTrack(0);
        }
        
        allEnemiesAreDead = true;
    }

    void Display_Strict_Restriction()
    {
        StrictStealth_UI = GameObject.FindGameObjectWithTag("StrictStealth");

        if (StrictStealth_UI != null)
        {
            if (StrictStealth)
            {
                StrictStealth_UI.SetActive(true);
            } else
                {
                    StrictStealth_UI.SetActive(false);
                }
        }
    }

    IEnumerator AlertWave()
    {
        botScripts = GameObject.FindObjectsOfType<robotAIScript>();

        foreach (robotAIScript bot in botScripts)
        {
            bot.AttackedByPlayer();
        }

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(AlertWave());
    }

    public bool isAreaOnAlert()
    {
        return areaAlerted;
    }

    public bool areAllEnemiesDead()
    {
        return allEnemiesAreDead;
    }

    public void AlertForBodyFound()
    {
        if (!allEnemiesAreDead)
        {
            if (inPrison)
            {
                if (GameObject.Find("PrisonAlert") != null)
                {
                    GameObject.Find("PrisonAlert").GetComponent<PrisonAlarm>().SecurityAlert();
                }

                AlertAllBots();
            }

            foreach (robotAIScript bot in botScripts)
            {
                bot.RobotBodyFound();
            }
        }
    }

    public void AlertAllBots()
    {
        if (!allEnemiesAreDead)
        {
            if (StrictStealth)
            {
                if (GameObject.Find("PlayerCam") != null && GameObject.Find("PlayerCam").GetComponent<AudioFades>() != null)
                {
                    GameObject.Find("PlayerCam").GetComponent<AudioFades>().FadeAudioOut();
                }

                GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
                GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex);
            } else
                {
                    areaAlerted = true;

                    if (!musicAltered)
                    {
                        if (GameObject.Find("PlayerCam") != null && GameObject.Find("PlayerCam").GetComponent<AudioFades>() != null)
                        {
                            GameObject.Find("PlayerCam").GetComponent<AudioFades>().ChangeMusicTrack(1);
                        }

                        musicAltered = true;
                    }
            
                    foreach (robotAIScript bot in botScripts)
                    {
                        bot.AttackedByPlayer();
                    }
                }
        }
    }

    public void DisableForceStealth()
    {
        StrictStealth = false;
    }
}//EndScript