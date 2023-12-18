using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFades : MonoBehaviour
{
    GameObject[] MusicComponent, SFXComponent;
    [Range(0, 1)] float currentMusicVolume = 0, currentSFXVolume = 0;
    bool LoadingScene = true, transitionToScene = false, changingTracks = false, mainTrackAtZero = false;
    [SerializeField] AudioSource MainMusic, AlertMusic, BossMusic;
    int trackValue;

    public void SetCurrents(float m, float s)
    {
        currentMusicVolume = m;
        currentSFXVolume = s;
    }

    void Awake()
    {
        MusicComponent = GameObject.FindGameObjectsWithTag("Music");
        SFXComponent = GameObject.FindGameObjectsWithTag("SFX");

        foreach (GameObject Sound in MusicComponent)
        {
            Sound.GetComponent<AudioSource>().volume = currentMusicVolume;
        }

        foreach (GameObject Sound in SFXComponent)
        {
            Sound.GetComponent<AudioSource>().volume = currentSFXVolume;
        }
    }

    public void ConfigForNewLevel()
    {
        MusicComponent = GameObject.FindGameObjectsWithTag("Music");
        SFXComponent = GameObject.FindGameObjectsWithTag("SFX");

        foreach (GameObject Sound in MusicComponent)
        {
            Sound.GetComponent<AudioSource>().volume = currentMusicVolume;
        }

        foreach (GameObject Sound in SFXComponent)
        {
            Sound.GetComponent<AudioSource>().volume = currentSFXVolume;
        }

        MainMusic.Play();

        LoadingScene = true;
        transitionToScene = false;
        changingTracks = false;
        mainTrackAtZero = false;
    }

    public void SetMainAudio(AudioClip music)
    {
        MainMusic.clip = music;
    }

    void Start() // music and sfx will fade in
    {
        MainMusic.Play();
    }

    void Update()
    {
        if (LoadingScene)
        {
            MusicComponent = GameObject.FindGameObjectsWithTag("Music");
            SFXComponent = GameObject.FindGameObjectsWithTag("SFX");

            BringMusicBack();
        }

        if (transitionToScene)
        {
            MusicComponent = GameObject.FindGameObjectsWithTag("Music");
            SFXComponent = GameObject.FindGameObjectsWithTag("SFX");
            
            BringDownMusic();
        }

        if (changingTracks)
        {
            MusicComponent = GameObject.FindGameObjectsWithTag("Music");
            SFXComponent = GameObject.FindGameObjectsWithTag("SFX");
            
            SwitchTheTracks();
        }
    }

    public void FadeAudioOut() // music n sfx will fade out
    {
        transitionToScene = true;
    }

//=======================================================================================================================================

    public void ChangeMusicTrack(int value) // current music will fade out and a new track will fade in
    {
        trackValue = value;
        changingTracks = true;
    }

//=======================================================================================================================================

    void BringMusicBack()
    {
        if (currentMusicVolume < PlayerPrefs.GetFloat("MusicVol"))
        {
            currentMusicVolume += 0.0115f;

            foreach (GameObject Sound in MusicComponent)
            {
                Sound.GetComponent<AudioSource>().volume = currentMusicVolume;
            }
        }

        if (currentSFXVolume < PlayerPrefs.GetFloat("SFXVol"))
        {
            currentSFXVolume += 0.0115f;

            foreach (GameObject Sound in SFXComponent)
            {
                Sound.GetComponent<AudioSource>().volume = currentSFXVolume;
            }
        }

        if (currentMusicVolume >= PlayerPrefs.GetFloat("MusicVol") && currentSFXVolume >= PlayerPrefs.GetFloat("SFXVol"))
        {
            LoadingScene = false;
        }
    }

    void BringDownMusic()
    {
        if (currentMusicVolume > 0)
        {
            currentMusicVolume -= 0.0115f;

            foreach (GameObject Sound in MusicComponent)
            {
                Sound.GetComponent<AudioSource>().volume = currentMusicVolume;
            }
        }

        if (currentSFXVolume > 0)
        {
            currentSFXVolume -= 0.0115f;

            foreach (GameObject Sound in SFXComponent)
            {
                Sound.GetComponent<AudioSource>().volume = currentSFXVolume;
            }
        }

        if (currentMusicVolume <= 0 && currentSFXVolume <= 0)
        {
            transitionToScene = false;
        }
    }

    void SwitchTheTracks()
    {
        if (mainTrackAtZero)
        {
            if (currentMusicVolume < PlayerPrefs.GetFloat("MusicVol"))
            {
                currentMusicVolume += 0.025f;

                foreach (GameObject Sound in MusicComponent)
                {
                    Sound.GetComponent<AudioSource>().volume = currentMusicVolume;
                }
            } else
                {
                    changingTracks = false;
                    mainTrackAtZero = false;
                }
        } else
            {
                if (currentMusicVolume > 0)
                {
                    currentMusicVolume -= 0.025f;

                    foreach (GameObject Sound in MusicComponent)
                    {
                        Sound.GetComponent<AudioSource>().volume = currentMusicVolume;
                    }
                } else
                    {
                        switch (trackValue)
                        {
                            case 0:
                                MainMusic.Play();
                                AlertMusic.Stop();
                                BossMusic.Stop();

                                mainTrackAtZero = true;
                            break;
                            case 1:
                                MainMusic.Stop();
                                AlertMusic.Play();
                                BossMusic.Stop();

                                mainTrackAtZero = true;
                            break;
                            case 2:
                                MainMusic.Stop();
                                AlertMusic.Stop();
                                BossMusic.Play();

                                mainTrackAtZero = true;
                            break;

                            default:
                            break;
                        }
                    }
            }
    }

}//EndScript