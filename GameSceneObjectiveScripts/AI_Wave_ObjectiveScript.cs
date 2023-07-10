using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AI_Wave_ObjectiveScript : MonoBehaviour
{
    [SerializeField] int NumberOfWaves, currentWave, NumberOfEnemiesPerWave, numberOfEnemiesSpawned;
    [SerializeField] GameObject AI_Pref;
    [SerializeField] Transform[] SpawnPoints;
    bool FirstWaveSpawned = false;

    void Start()
    {
        StartCoroutine(FirstSpawn());
    }

    IEnumerator FirstSpawn()
    {
        yield return new WaitForSeconds(1.5f);
        SpawningAI();
        FirstWaveSpawned = true;
    }

    void Update()
    {
        if (currentWave < NumberOfWaves)
        {
            robotAIScript[] botScripts = GameObject.FindObjectsOfType<robotAIScript>();
    
            int bodyCount = 0;
    
            for (int i = 0; i < botScripts.Length; i++)
            {
                if (!botScripts[i].enabled)
                {
                    bodyCount++;
                }
            }
    
            if (bodyCount >= botScripts.Length && FirstWaveSpawned)
            {
                SpawningAI();
            }
        } else
            {
                GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
                EnterNextLevel();
            }

        SetBotsToAggressive();
    }

    void SetBotsToAggressive()
    {
        robotAIScript[] botScripts = GameObject.FindObjectsOfType<robotAIScript>();

        foreach (robotAIScript robot in botScripts)
        {
            robot.AttackedByPlayer();
        }
    }

    void EnterNextLevel()
    {
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void SpawningAI()
    {
        for (numberOfEnemiesSpawned = 0; numberOfEnemiesSpawned < NumberOfEnemiesPerWave;)
        {
            for (int i = 0; i < SpawnPoints.Length; i++)
            {
                if (!SpawnPoints[i].GetComponent<AI_WaveSpawner>().isPlayerInArea())
                {
                    Instantiate(AI_Pref, SpawnPoints[i].position, SpawnPoints[i].rotation);

                    numberOfEnemiesSpawned++;
                }
            }
        }

        currentWave++;
    }
}//EndScript