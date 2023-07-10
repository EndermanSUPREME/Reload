using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneTransitionScript : MonoBehaviour
{
    public float LengthOfCutscene, StartFadeTime;
    public Animator FadeAnim;
    public Text skipTxt;

    void Start()
    {
        Invoke("GoToNextScene", LengthOfCutscene);
        
        Invoke("FadeToBlack", StartFadeTime);
    }

    void Update()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            skipTxt.text = "X Button To Skip";
        } else
            {
                skipTxt.text = "Space To Skip";
            }

        if (Input.GetKeyDown(KeyCode. Space) || Input.GetKeyDown("joystick button 2")) // spacebar or X button
        {
            FadeToBlack();
            Invoke("GoToNextScene", 2f);
        }
    }

    void FadeToBlack()
    {
        FadeAnim.Play("FadeIn"); // scene isnt triggered
    }

    void GoToNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}//EndScript