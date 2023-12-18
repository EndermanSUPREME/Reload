using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitioning : MonoBehaviour
{
    Animator ScreenFader;
    int sceneInt;

    void Start()
    {
        if (transform.name != "LevelWarp")
        {
            ScreenFader = GetComponent<Animator>();
            
            if (SceneManager.GetActiveScene().buildIndex != 12) // CTO Scene
            {
                FadeOut();
            }
        }
    }

    public void FadeIn() // Changes Scene
    {
        if (GameObject.FindObjectOfType<UI_Interface>() != null) GameObject.FindObjectOfType<UI_Interface>().enabled = false;

        if (ScreenFader != null)
        {
            ScreenFader.Play("FadeIn_Event");
        }
    }

    public void FadeInAndAdjustExternalMusic() // Changes Scene and fade audio from prison scenes to the end
    {
        AudioFades AudioShifter = (AudioFades)FindObjectOfType(typeof(AudioFades));
        
        if (AudioShifter != null)
        {
            AudioShifter.FadeAudioOut();
        }
    }

    public void FadeIn_Event() // Doesnt Change Scene
    {
        if (GameObject.FindObjectOfType<UI_Interface>() != null) GameObject.FindObjectOfType<UI_Interface>().enabled = false;

        if (ScreenFader != null)
        {
            ScreenFader.Play("FadeIn");
        }
    }

    public void FadeOut()
    {
        if (ScreenFader != null)
        {
            ScreenFader.Play("FadeOut");

            if (GameObject.FindObjectOfType<UI_Interface>() != null) GameObject.FindObjectOfType<UI_Interface>().enabled = true;
        }
    }
    
//=============================================================================

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneInt);
    }

    public void SetNextInt(int newInt)
    {
        sceneInt = newInt;
    }

//=============================================================================

    public void LoadWithCurrentPlayer()
    {
        FadeInAndAdjustExternalMusic();
        GameObject.Find("playerBody").GetComponent<GoToNextScene>().TestDontDestroy();
    }

}//EndScript