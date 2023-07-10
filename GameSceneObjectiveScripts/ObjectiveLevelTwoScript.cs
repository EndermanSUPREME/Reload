using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveLevelTwoScript : MonoBehaviour
{
    [SerializeField] Material buttonRed, buttonGreen;
    [SerializeField] Renderer lightOne, lightTwo, elevatorButton;
    [SerializeField] bool leversOpen = false, switchOff = false, switchOff2 = false;
    [SerializeField] Animator switchOne, switchTwo;

    void Start()
    {
        lightOne.material = buttonGreen;
        lightTwo.material = buttonGreen;

        elevatorButton.material = buttonRed;
        GetComponent<Collider>().enabled = false;
    }

    void Update()
    {
        CheckSwitchStatus();
    }

    public void DisableSwitchOne()
    {
        switchOne.Play("leverDown");
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        lightOne.material = buttonRed;
        switchOff = true;
    }

    public void DisableSwitchTwo()
    {
        switchTwo.Play("leverDown");
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        lightTwo.material = buttonRed;
        switchOff2 = true;
    }

    void CheckSwitchStatus()
    {
        if (switchOff && switchOff2)
        {
            leversOpen = true;
            elevatorButton.material = buttonGreen;
            GetComponent<Collider>().enabled = true;
        }
    }

    public void GoToLevelThree()
    {
        if (leversOpen)
        {
            GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

}//EndScript