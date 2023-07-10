using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class configNewLevelAudio : MonoBehaviour
{
    public AudioClip MainForLevel;
    bool audioAdjusted = false;

    void Update()
    {
        if (!audioAdjusted)
        {
            UI_Interface UI_Settings = (UI_Interface)FindObjectOfType(typeof(UI_Interface));
            AudioFades AudioShifter = (AudioFades)FindObjectOfType(typeof(AudioFades));

            if (UI_Settings != null && AudioShifter != null)
            {
                UI_Settings.ConfigForNewLevel();
        
                AudioShifter.SetMainAudio(MainForLevel);
                AudioShifter.ConfigForNewLevel();

                audioAdjusted = true;
            }
        }
    }
}//EndScript