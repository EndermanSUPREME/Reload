using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonAlarm : MonoBehaviour
{
    [SerializeField] GameObject[] AlarmLights;
    [SerializeField] bool PrisonOnAlert = false, PodGaurdDead = false, triggeredAlarm = false;
    [SerializeField] AudioSource prisonAlarmSound;

    void Start()
    {
        foreach (GameObject Light in AlarmLights)
        {
            Light.SetActive(false);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>().isAreaOnAlert() && !triggeredAlarm)
        {
            SecurityAlert();
        }

        if (PrisonOnAlert && !triggeredAlarm)
        {
            foreach (GameObject AlarmLight in AlarmLights)
            {
                AlarmLight.transform.Rotate(0, 0, 5);
            }

            SecurityAlert();
        }
    }

    public void SecurityAlert()
    {
        if (!triggeredAlarm)
        {
            triggeredAlarm = true;

            PrisonOnAlert = true;

            prisonAlarmSound.Play();

            if (GameObject.Find("PlayerCam") != null && GameObject.Find("PlayerCam").GetComponent<AudioFades>() != null)
            {
                GameObject.Find("PlayerCam").GetComponent<AudioFades>().FadeAudioOut();
            }

            GameObject.Find("PlayerCam").layer = 7;

            GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>().AlertAllBots();

            foreach (GameObject Light in AlarmLights)
            {
                Light.SetActive(true);
            }
        }
    }

    public void PodGaurdDown()
    {
        PodGaurdDead = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.name == "PlayerCam")
        {
            print("[+] Player Entered Trigger : " + PodGaurdDead);
            if (!PodGaurdDead && !triggeredAlarm)
            {
                SecurityAlert();
                triggeredAlarm = true;
            }
        }
    }

}//EndScript