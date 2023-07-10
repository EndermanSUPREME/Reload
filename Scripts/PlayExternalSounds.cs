using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayExternalSounds : MonoBehaviour
{
    public string soundName;
    AudioSource Sound;

    void Start()
    {
        Sound = GameObject.Find(soundName).GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        if (Sound != null)
        {
            Sound.Play();
        }
    }
}//EndScript