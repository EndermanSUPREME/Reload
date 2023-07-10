using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroll : MonoBehaviour
{
    Animator CreditTextAnim;

    void Start()
    {
        if (GameObject.Find("playerBody") != null)
        {
            Destroy(GameObject.Find("playerBody"));
        }

        CreditTextAnim = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode. Space))
        {
            CreditTextAnim.speed = 3;
        } else
            {
                CreditTextAnim.speed = 1;
            }
    }

    public void ReturnToMainMenuScene()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        AudioFades AudioShifter = (AudioFades)FindObjectOfType(typeof(AudioFades));

        if (AudioShifter != null)
        {
            AudioShifter.FadeAudioOut();
        }

        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(0);
    }

    public void ViewLink()
    {
        System.Diagnostics.Process.Start("https://shadowwolf0621.itch.io/");
    }
}//EndScript