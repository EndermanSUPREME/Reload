using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelOneScript : MonoBehaviour
{
    [SerializeField] Renderer elevatorButton;
    [SerializeField] Material buttonGreen, buttonRed;
    [SerializeField] GameObject securityDongle;

    void Start()
    {
        elevatorButton.material = buttonRed;
        securityDongle.SetActive(true);
        GetComponent<Collider>().enabled = false;
    }
    
    void Update()
    {
        if (!securityDongle.activeSelf)
        {
            elevatorButton.material = buttonGreen;
            GetComponent<Collider>().enabled = true;
        }
    }

    public void EnterNextLevel()
    {
        if (!securityDongle.activeSelf)
        {
            GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}//EndScript