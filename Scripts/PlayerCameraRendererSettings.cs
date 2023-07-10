using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRendererSettings : MonoBehaviour
{
    int screenRendIndex = 0;
    [SerializeField] GameObject[] RobotVisionObjects;
    [SerializeField] RenderTexture robotVisionTexture;

    void Start()
    {
        screenRendIndex = PlayerPrefs.GetInt("RenderInt");
        
        switch (screenRendIndex)
        {
            case 1:
                EngageRobotVision();
            break;

            default:
                screenRendIndex = 0;
                DefaultVision();
            break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode. V) || Input.GetKeyDown("joystick button 3")) // V key or Y Button
        {
            ChangeScreenRenderer();
        }
    }

    public void ChangeScreenRenderer()
    {
        screenRendIndex++;
    
        switch (screenRendIndex)
        {
            case 1:
                EngageRobotVision();
            break;

            default:
                screenRendIndex = 0;
                DefaultVision();
            break;
        }

        PlayerPrefs.SetInt("RenderInt", screenRendIndex);
    }

    void DefaultVision()
    {
        GameObject.Find("PlayerCam").GetComponent<Camera>().targetTexture = null; // normal
        GameObject.Find("PlayerCam").GetComponent<Camera>().cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "ground", "Water", "UI", "Player", "DeadBody", "Robot", "Limb", "PlayerPrisoner");

        for (int i = 0; i < RobotVisionObjects.Length; i++)
        {
            RobotVisionObjects[i].SetActive(false);
        }
    }

    void EngageRobotVision()
    {
        GameObject.Find("PlayerCam").GetComponent<Camera>().targetTexture = robotVisionTexture; // vision
        GameObject.Find("PlayerCam").GetComponent<Camera>().cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "ground", "Water", "UI", "Player", "DeadBody", "Robot", "Limb", "PlayerPrisoner", "HighLight");

        for (int i = 0; i < RobotVisionObjects.Length; i++)
        {
            RobotVisionObjects[i].SetActive(true);
        }
    }
}//EndScript