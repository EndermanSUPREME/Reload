using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonAlertBarManager : MonoBehaviour
{
    prisonCCTV_Camera[] prisonCams;
    robotAIScript[] gaurds;

    // Start is called before the first frame update
    void Start()
    {
        prisonCams = GameObject.FindObjectsOfType<prisonCCTV_Camera>();
        gaurds = GameObject.FindObjectsOfType<robotAIScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (prisonCams.Length > 0)
        {
            CheckingCCTVCameras();
        }

        if (gaurds.Length > 0)
        {
            CheckingRobotGaurds();
        }
    }

    void CheckingCCTVCameras()
    {
        int k = 0;
        for (int i = 0; i < prisonCams.Length; i++)
        {
            if (prisonCams[i].isWatchingPlayer())
            {
                // Debug.Log("[-] Camera Watching the Player!");
                return;
            }

            k++;
        }

        // Debug.Log("[+] Done with Loop");
        // Debug.Log("[+] k = " + k.ToString());

        if (k == prisonCams.Length)
        {
            prisonCams[0].ResetCameraDetectionMeter();
        }
    }

    void CheckingRobotGaurds()
    {
        int k = 0;
        for (int i = 0; i < gaurds.Length; i++)
        {
            if (gaurds[i].RobotSeesPlayer())
            {
                return;
            }

            k++;
        }

        if (k == gaurds.Length)
        {
            gaurds[0].ResetRobotDetectionMeter();
        }
    }

}//EndScript