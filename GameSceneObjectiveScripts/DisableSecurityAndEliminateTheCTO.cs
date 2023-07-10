using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableSecurityAndEliminateTheCTO : MonoBehaviour
{
    bool switchOne = false, switchTwo = false, switchThree = false, switchFour = false, ObjectivesCompleted = false;
    [SerializeField] Renderer switchLight, switchLight2, switchLight3, switchLight4;
    [SerializeField] Animator switchAnim, switchAnim2, switchAnim3, switchAnim4;
    public Material red, green;
    [SerializeField] Transform playerPositionAfterCutscene;
    Transform playerCameraObj, playerBodyObject;
    [SerializeField] GameObject CTO, CTO_Cutscene, CTO_Cutscene_2, BossFightBarrier, ExitBlockade, CTO_Office_Pointer, CTO_OfficeDoor;
    AudioFades audioInterface;

    void Awake()
    {
        Invoke("StartLevel", 0.05f);
    }

    void StartLevel()
    {
        playerCameraObj = GameObject.Find("PlayerCam").transform;
        playerBodyObject = GameObject.Find("playerBody").transform;

        if (playerCameraObj != null && playerBodyObject != null)
        {
            if (PlayerPrefs.GetInt("InCTO_Fight") == 0)
            {
                Invoke("FadeScreenOut", 0.25f);

                if (CTO != null)
                {
                    CTO.SetActive(false);
                    CTO_OfficeDoor.SetActive(true);

                    if (CTO_Cutscene != null)
                    {
                        CTO_Cutscene.SetActive(false);
                        CTO_Cutscene_2.SetActive(false);
                    }
                }

                CTO_Office_Pointer.SetActive(false);

                switchLight.material = green;
                switchLight2.material = green;
                switchLight3.material = green;
                switchLight4.material = green;

                BossFightBarrier.SetActive(false);
            }

            if (PlayerPrefs.GetInt("InCTO_Fight") == 1)
            {
                // Invoke("FadeScreenOut", 0.25f);

                playerCameraObj.GetComponent<AudioListener>().enabled = false;

                if (CTO != null)
                {
                    CTO.SetActive(false);
                    CTO_OfficeDoor.SetActive(true);

                    if (CTO_Cutscene != null)
                    {
                        CTO_Cutscene.SetActive(false);
                        CTO_Cutscene_2.SetActive(false);
                    }
                }

                switchLight.material = red;
                switchLight2.material = red;
                switchLight3.material = red;
                switchLight4.material = red;

                BossFightBarrier.SetActive(false);

                ReloadBossFight();
            }
        } else
            {
                Invoke("StartLevel", 0.05f);
            }
    }

    void GiveReloadedPlayerTheirWeaponsFromSaveFile()
    {
        print("[+] Gift Player Weapons");
        GameObject.Find("PlayerCam").transform.GetComponent<Weapons>().GiveGearFromSave();
    }

    void FadeScreenOut()
    {
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeOut();
    }

    void Update()
    {
        if (ObjectivesCompleted)
        {
            ExitBlockade.SetActive(false);
            CTO_Office_Pointer.SetActive(true);
            CTO_Office_Pointer.transform.localPosition = new Vector3(18, 44, 13);
        } else
            {
                ExitBlockade.SetActive(true);

                if (switchOne && switchTwo && switchThree && switchFour) // only triggers when all switches are flipped
                {
                    CTO_Office_Pointer.SetActive(true);
                    CTO_Office_Pointer.transform.localPosition = new Vector3(-56.9003448f, 38.7200012f, -1.67999995f);
                }
            }
    }

    public void TurnOff_SwitchOne()
    {
        switchAnim.Play("leverDown");
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        switchLight.material = red;
        switchOne = true;
    }

    public void TurnOff_SwitchTwo()
    {
        switchAnim2.Play("leverDown");
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        switchLight2.material = red;
        switchTwo = true;
    }

    public void TurnOff_SwitchThree()
    {
        switchAnim3.Play("leverDown");
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        switchLight3.material = red;
        switchThree = true;
    }

    public void TurnOff_SwitchFour()
    {
        switchAnim4.Play("leverDown");
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        switchLight4.material = red;
        switchFour = true;
    }

//==================================== Boss Fight Trigger =================================================

    public void ReloadBossFight()
    {
        // Boss Fight Commence
        AudioFades AudioShifter = (AudioFades)FindObjectOfType(typeof(AudioFades));
        
        if (AudioShifter != null)
        {
            AudioShifter.FadeAudioOut();
        }
        
        PlayerPrefs.SetInt("InCTO_Fight", 1);
        GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>().enabled = false;

        transform.GetComponent<Collider>().enabled = false;
        playerBodyObject.GetComponent<movement>().enabled = false;

        Invoke("PlayBossStartCutscene", 2); // length of fade
    }

    void OnTriggerEnter(Collider collider)
    {
        if (switchOne && switchTwo && switchThree && switchFour) // only triggers when all switches are flipped
        {
            if (collider.transform == playerCameraObj)
            {
                // Boss Fight Commence
                PlayerPrefs.SetInt("InCTO_Fight", 1);
                GameObject.Find("PlayerCam").GetComponent<SaveGameData>().BuildSaveFile(); // save progress in level
                GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn_Event();

                ReloadBossFight();
            }
        }
    }

    public void TestBossFight()
    {
        // Boss Fight Commence
        AudioFades AudioShifter = (AudioFades)FindObjectOfType(typeof(AudioFades));
        
        if (AudioShifter != null)
        {
            AudioShifter.FadeAudioOut();
        }

        PlayerPrefs.SetInt("InCTO_Fight", 1);
        GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>().enabled = false;

        transform.GetComponent<Collider>().enabled = false;
        playerBodyObject.GetComponent<movement>().enabled = false;

        Invoke("PlayBossStartCutscene", 2); // length of fade
    }

    void PlayBossStartCutscene()
    {
        if (playerCameraObj != null)
        {
            playerCameraObj.gameObject.SetActive(false);
            playerCameraObj.GetComponent<AudioListener>().enabled = true;
        }

        CTO_Office_Pointer.SetActive(false);
        Destroy(GameObject.Find("Level_AI"));

        if (CTO != null)
        {
            CTO_Cutscene.SetActive(true);
        }

        BossFightBarrier.SetActive(true);

        if (playerBodyObject != null)
        {
            playerBodyObject.gameObject.SetActive(false);
            playerBodyObject.transform.position = playerPositionAfterCutscene.position;
            playerBodyObject.gameObject.SetActive(true);
        }

        Invoke("MovePlayerIntoBossFightArea", 17.1f); // length of cutscene
    }

    void MovePlayerIntoBossFightArea()
    {
        print("[*] Repositioning Player");

        if (playerBodyObject != null)
        {
            playerBodyObject.transform.position = playerPositionAfterCutscene.position;
        }

        if (playerBodyObject.transform.position == playerPositionAfterCutscene.position)
        {
            print("[+] Player Ready!");
            BeginFight();
        } else
            {
                Invoke("MovePlayerIntoBossFightArea", 0.001f);
            }
    }

    void BeginFight()
    {
        BossFightBarrier.SetActive(true);

        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();

        if (playerCameraObj != null)
        {
            playerCameraObj.gameObject.SetActive(true);
        }

        if (CTO != null)
        {
            CTO.SetActive(true);
        }

        if (CTO_Cutscene != null)
        {
            CTO_Cutscene.SetActive(false);
        }

        if (GameObject.Find("PlayerCam") != null)
        {
            audioInterface = GameObject.Find("PlayerCam").GetComponent<AudioFades>();
        
            if (audioInterface != null)
            {
                audioInterface.ChangeMusicTrack(2);
            }
        }

        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeOut();
        playerBodyObject.GetComponent<movement>().enabled = true;

        GiveReloadedPlayerTheirWeaponsFromSaveFile();
    }

//============================================== CTO Death ====================================================

    public void CTO_HasDied()
    {
        GameObject.Find("PlayerCam").GetComponent<Collider>().enabled = false;

        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn_Event();
        playerBodyObject.GetComponent<movement>().enabled = false;

        Invoke("PlayBossEndCutscene", 2); // length of fade

        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        ObjectivesCompleted = true;
    }

    void PlayBossEndCutscene()
    {
        if (playerCameraObj != null)
        {
            playerCameraObj.gameObject.SetActive(false);
        }

        if (CTO != null)
        {
            CTO_Cutscene_2.SetActive(true);
        }

        Invoke("ReloadMapAsNormal", 8.1f); // length of cutscene
    }

    void ReloadMapAsNormal()
    {
        if (CTO != null)
        {
            CTO_Cutscene_2.SetActive(false);
            CTO_OfficeDoor.SetActive(false);
        }

        if (playerCameraObj != null)
        {
            playerCameraObj.gameObject.SetActive(true);
        }

        GameObject.Find("PlayerCam").GetComponent<Collider>().enabled = true;

        BossFightBarrier.SetActive(false);

        audioInterface = GameObject.Find("PlayerCam").GetComponent<AudioFades>();
        audioInterface.ChangeMusicTrack(0);

        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeOut();
        playerBodyObject.GetComponent<movement>().enabled = true;
    }

    public void GoToHelipad()
    {
        playerBodyObject.GetComponent<GoToNextScene>().GoingToNextLevel();
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex + 1);
    }

}//EndScript