using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempMute : MonoBehaviour
{
    void Awake()
    {
        GetComponent<AudioSource>().mute = true;
    }

    void Start()
    {
        Invoke("unMuteAudioSource", 0.5f);
    }

    void unMuteAudioSource()
    {
        GetComponent<AudioSource>().mute = false;
    }
}//EndScript