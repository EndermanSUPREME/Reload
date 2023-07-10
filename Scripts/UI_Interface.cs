using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Interface : MonoBehaviour
{
    private float musicVolume, sfxVolume, ResolutionIndex;
    [SerializeField] int menuIndex = 0, sliderSelectedIndex = 0;
    [SerializeField] Button[] mainMenuButtons, settingsButtons;
    bool GamePaused = false, debugFrames = false, controllerPause = false, dpadPress = false, dpadPress2 = false, bodyHasBeenFound = false;
    private int[] ScreenWidth = {800, 1024, 1152, 1280, 1280, 1440, 1600, 1920, 2560}, ScreenHeight = {600, 768, 864, 720, 1024, 900, 1200, 1080, 1440}; // [0, 8]
    [SerializeField] Slider MusicSlider, SfxSlider, ResolutionSlider, Sensitivity_Slider;
    [SerializeField] Image[] sliderKnobs;
    [SerializeField] Sprite unSelected, Selected;
    GameObject[] MusicComponent, SFXComponent;
    [SerializeField] GameObject HUD, Main, Setting;
    [SerializeField] Text MusicDisplay, SFX_Display, ResolutionDisplay, PlayerWarningText, ObjectiveDisplay, FPS_Display, Sensitivity_Display;
    public Weapons playerWeaponScript;

    void Start()
    {
        Application.targetFrameRate = 65;
        
        if (Setting != null)
        {
            GamePaused = false;

            if (SceneManager.GetActiveScene().buildIndex > 0)
            {
                ResumeGame();
            } else
                {
                    GoToMain();
                }
    
            // Get all objects in the scene that have a script for editing
            MusicComponent = GameObject.FindGameObjectsWithTag("Music");
            SFXComponent = GameObject.FindGameObjectsWithTag("SFX");
    
            ApplyPlayerSettings();
        }
    }

    public void ConfigForNewLevel()
    {
        Application.targetFrameRate = 65;
        
        if (Setting != null)
        {
            GamePaused = false;

            if (SceneManager.GetActiveScene().buildIndex > 0)
            {
                ResumeGame();
            } else
                {
                    GoToMain();
                }
    
            // Get all objects in the scene that have a script for editing
            MusicComponent = GameObject.FindGameObjectsWithTag("Music");
            SFXComponent = GameObject.FindGameObjectsWithTag("SFX");
    
            ApplyPlayerSettings();
        }
    }

    void Update()
    {
        if (GameObject.Find("BotNetAlert") != null)
        {
            if (ObjectiveDisplay != null)
            {
                ObjectiveDisplay.text = GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().GetCurrentObjective();
            }
    
            if (!GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>().areAllEnemiesDead())
            {
                if (!GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>().isAreaOnAlert() && !bodyHasBeenFound) // no alerts
                {
                    PlayerWarningText.text = "";
                }

                if (!GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>().isAreaOnAlert() && bodyHasBeenFound) // bodyFound
                {
                    PlayerWarningText.text = "Body_Found";
                }
            } else
                {
                    PlayerWarningText.text = "All Enemies Killed";
                }
        }

        if (Setting.active && Setting != null)
        {
            ResolutionIndex = ResolutionSlider.value;
            MusicDisplay.text = MusicSlider.value * 10 + " %";
            SFX_Display.text = SfxSlider.value * 10 + " %";
            ResolutionDisplay.text = ScreenWidth[(int)ResolutionIndex] + "x" + ScreenHeight[(int)ResolutionIndex];
            Sensitivity_Display.text = Sensitivity_Slider.value.ToString();

            SetAudioSettings();
            SetSensitivitySettings();
        }

        if (SceneManager.GetActiveScene().buildIndex > 0 && SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            if (Setting != null)
            {
                bool Escape = Input.GetKeyDown(KeyCode. Escape);
                bool PauseButtonOnRemote = Input.GetButtonDown("remotePause");

                int currentSceneInt = SceneManager.GetActiveScene().buildIndex;
    
                if (currentSceneInt != 10 || currentSceneInt != 14)
                {
                    if (Escape)
                    {
                        if (!GamePaused)
                        {
                            if (playerWeaponScript != null)
                            {
                                playerWeaponScript.enabled = false;
                            }

                            GoToSettings();
                            GamePaused = true;
                        } else
                            {
                                ResumeGame();
                                GamePaused = false;
                            }
                    }

                    if (PauseButtonOnRemote)
                    {
                        if (!controllerPause)
                        {
                            if (playerWeaponScript != null)
                            {
                                playerWeaponScript.enabled = false;
                            }

                            GoToSettings();
                            controllerPause = true;
                        } else
                            {
                                ResumeGame();
                                controllerPause = false;
                            }
                    }
                }

                if (GamePaused || controllerPause)
                {
                    Time.timeScale = 0;
                    GetComponent<AudioFades>().enabled = false;
                    
                    if (transform.GetComponent<mouse>() != null)
                    {
                        transform.GetComponent<mouse>().enabled = false;
                        transform.GetComponent<Weapons>().enabled = false;
                    }

                    if (controllerPause)
                    {
                        Controller_Navigation();
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;

                        PlayerPrefs.SetInt("isGamePaused", 1);
                    }
                }

                if (!GamePaused && !controllerPause)
                {
                    Time.timeScale = 1;
                    GetComponent<AudioFades>().enabled = true;
                    Invoke("AllowJumpMovementForPlayer", 0.05f);

                    if (transform.GetComponent<mouse>() != null)
                    {
                        transform.GetComponent<mouse>().enabled = true;
                        transform.GetComponent<Weapons>().enabled = true;
                    }

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    PlayerPrefs.SetInt("isGamePaused", 0);
                }
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 0 && Input.GetJoystickNames().Length > 0) // in main menu we can nav with remote
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Controller_Navigation();
        } else if (SceneManager.GetActiveScene().buildIndex == 0 && Input.GetJoystickNames().Length < 1) // we use mouse to navigate
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }

        if (FPS_Display != null)
        {
            if (debugFrames)
            {
                float fpsNum = Mathf.Round(1.0f / Time.deltaTime);
                FPS_Display.text = fpsNum.ToString() + " : FPS";
            } else
                {
                    FPS_Display.text = "";
                }
        }
    }

    void AllowJumpMovementForPlayer()
    {
        PlayerPrefs.SetInt("isGamePaused", 0);
    }

    void Controller_Navigation()
    {
        // print("Remote Access. . .");

        if (Input.GetAxis("DPad Y") == 0)
        {
            dpadPress = false;
        }

        if (!dpadPress && Input.GetAxis("DPad Y") > 0) // up
        {
            dpadPress = true;
            menuIndex++;
        }

        if (!dpadPress && Input.GetAxis("DPad Y") < 0) // down
        {
            dpadPress = true;
            menuIndex--;
        }


        if (Input.GetKeyDown("joystick button 4")) // L_bumper
        {
            sliderSelectedIndex--;
        }

        if (Input.GetKeyDown("joystick button 5")) // R_bumper
        {
            sliderSelectedIndex++;
        }

//=================================================================================//

        if (!Setting.active) // cycle in main menu buttons
        {
            if (menuIndex > -1 && menuIndex < mainMenuButtons.Length)
            {
                mainMenuButtons[menuIndex].Select();
            }

            if (menuIndex < 0)
            {
                menuIndex = mainMenuButtons.Length - 1;
            } else if (menuIndex >= mainMenuButtons.Length)
                {
                    menuIndex = 0;
                }
        } else // cycle settings buttons
            {
                if (menuIndex > -1 && menuIndex < settingsButtons.Length)
                {
                    settingsButtons[menuIndex].Select();
                }

                if (menuIndex < 0)
                {
                    menuIndex = settingsButtons.Length - 1;
                } else if (menuIndex >= settingsButtons.Length)
                    {
                        menuIndex = 0;
                    }

                Slider[] sliders = {MusicSlider, SfxSlider, ResolutionSlider, Sensitivity_Slider};

                if (sliderSelectedIndex >= 0 && sliderSelectedIndex <= 3)
                {
                    if (Input.GetAxis("DPad X") == 0)
                    {
                        dpadPress2 = false;
                    }

                    if (!dpadPress2 && Input.GetAxis("DPad X") > 0) // Right
                    {
                        dpadPress2 = true;
                        sliders[sliderSelectedIndex].value++;
                    }

                    if (!dpadPress2 && Input.GetAxis("DPad X") < 0) // Left
                    {
                        dpadPress2 = true;
                        sliders[sliderSelectedIndex].value--;
                    }

                    if (sliderSelectedIndex == 0)
                    {
                        sliderKnobs[0].sprite = Selected;
                        sliderKnobs[1].sprite = unSelected;
                        sliderKnobs[2].sprite = unSelected;
                        sliderKnobs[3].sprite = unSelected;
                    }

                    if (sliderSelectedIndex == 1)
                    {
                        sliderKnobs[0].sprite = unSelected;
                        sliderKnobs[1].sprite = Selected;
                        sliderKnobs[2].sprite = unSelected;
                        sliderKnobs[3].sprite = unSelected;
                    }

                    if (sliderSelectedIndex == 2)
                    {
                        sliderKnobs[0].sprite = unSelected;
                        sliderKnobs[1].sprite = unSelected;
                        sliderKnobs[2].sprite = Selected;
                        sliderKnobs[3].sprite = unSelected;
                    }

                    if (sliderSelectedIndex == 3)
                    {
                        sliderKnobs[0].sprite = unSelected;
                        sliderKnobs[1].sprite = unSelected;
                        sliderKnobs[2].sprite = unSelected;
                        sliderKnobs[3].sprite = Selected;
                    }
                }

                if (sliderSelectedIndex < 0)
                {
                    sliderSelectedIndex = 3;
                } else if (sliderSelectedIndex > 3)
                    {
                        sliderSelectedIndex = 0;
                    }
            }
    }

//========================== GAME SETTINGS =====================================
    public void ApplyPlayerSettings()
    {
        Screen.SetResolution(PlayerPrefs.GetInt("Width"), PlayerPrefs.GetInt("Height"), true);

        MusicSlider.value = PlayerPrefs.GetFloat("MusicVol") * 10;
        SfxSlider.value = PlayerPrefs.GetFloat("SFXVol") * 10;
        ResolutionSlider.value = PlayerPrefs.GetFloat("ResSlideVal");

        Sensitivity_Slider.value = ((float) PlayerPrefs.GetInt("Sensitivity"));
    }

    public void Set_SettingsFromFile(int rI, int mI, int sfxI, int s) // ================== Get Save Info ======================
    {
        ResolutionSlider.value = rI;
        MusicSlider.value = mI;
        SfxSlider.value = sfxI;
        Sensitivity_Slider.value = (float) s;

        SetResolutionSettings();
        SetAudioSettings();
        Sensitivity_Slider.value = s;

        Invoke("SetSensitivitySettings", 0.15f);
    }

    public void SetResolutionSettings()
    {
        ResolutionIndex = ResolutionSlider.value;
        Screen.SetResolution(ScreenWidth[(int)ResolutionIndex], ScreenHeight[(int)ResolutionIndex], true);

        PlayerPrefs.SetInt("Width", ScreenWidth[(int)ResolutionIndex]);
        PlayerPrefs.SetInt("Height", ScreenHeight[(int)ResolutionIndex]);
        PlayerPrefs.SetFloat("ResSlideVal", ResolutionSlider.value);
    }

    void SetSensitivitySettings()
    {
        if (GetComponent<mouse>() != null)
        {
            GetComponent<mouse>().SetSensitivity(Sensitivity_Slider.value);
        }

        PlayerPrefs.SetInt("Sensitivity", (int) (Sensitivity_Slider.value));
    }

    private void SetAudioSettings()
    {
        MusicComponent = GameObject.FindGameObjectsWithTag("Music");
        SFXComponent = GameObject.FindGameObjectsWithTag("SFX");

        musicVolume = MusicSlider.value / 10;
        sfxVolume = SfxSlider.value / 10;

        PlayerPrefs.SetFloat("MusicVol", musicVolume);
        PlayerPrefs.SetFloat("SFXVol", sfxVolume);

        PlayerPrefs.SetFloat("MusicSlideVal", MusicSlider.value);
        PlayerPrefs.SetFloat("SFXSlideVal", SfxSlider.value);

        if (MusicComponent != null && SFXComponent != null)
        {
            foreach (GameObject Sound in MusicComponent)
            {
                Sound.GetComponent<AudioSource>().volume = musicVolume;
            }
    
            foreach (GameObject Sound in SFXComponent)
            {
                Sound.GetComponent<AudioSource>().volume = sfxVolume;
            }
        }

        if (GetComponent<AudioFades>() != null)
        {
            GetComponent<AudioFades>().SetCurrents(musicVolume, sfxVolume);
        }
    }

    public void BodyFound()
    {
        bodyHasBeenFound = true;
        PlayerWarningText.text = "Body_Found";
    }

    public void PlayerSpotted()
    {
        PlayerWarningText.text = "Detected";
    }
    
//======================= SCENE CHANGES ==========================
    public void StartTheGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReturnToMainMenuScene()
    {
        ResumeGame();
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        GetComponent<Collider>().enabled = false;

        Destroy(GameObject.Find("playerBody"), 2);

        AudioFades AudioShifter = (AudioFades)FindObjectOfType(typeof(AudioFades));
        
        if (AudioShifter != null)
        {
            AudioShifter.FadeAudioOut();
        }

        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(0);
    }

    public void CloseTheGame()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            ResumeGame();
            GetComponent<Collider>().enabled = false;
        }

        AudioFades AudioShifter = (AudioFades)FindObjectOfType(typeof(AudioFades));
        
        if (AudioShifter != null)
        {
            AudioShifter.FadeAudioOut();
        }

        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn_Event();
        Invoke("CloseApplication", 2.1f);
    }

    void CloseApplication()
    {
        Application.Quit();
    }

//======================= SCREEN CHANGES ==========================
    public void ShowFrameRate()
    {
        if (!debugFrames)
        {
            debugFrames = true;
        } else
            {
                debugFrames = false;
            }
    }

    public void GoToPauseMenu() // InGame Scenes
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        HUD.SetActive(false);
        Setting.SetActive(false);
    }

    public void GoToMain() // Main Menu Scene
    {
        Time.timeScale = 1;
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        menuIndex = 0;

        Main.SetActive(true);
        Setting.SetActive(false);
    }

    public void GoToSettings()
    {
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        menuIndex = 0;

        HUD.SetActive(false);
        Setting.SetActive(true);
    }

    public void ResumeGame() // InGame Scenes
    {
        Time.timeScale = 1;

        GamePaused = false;
        controllerPause = false;

        if (playerWeaponScript != null)
        {
            playerWeaponScript.enabled = true;
        }

        Setting.SetActive(false);
        HUD.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}//EndScript