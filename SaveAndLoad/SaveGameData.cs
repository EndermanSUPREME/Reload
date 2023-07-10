using System; // convert.To lib
using System.IO; // File Lib
using System.Security.Cryptography; // encyption lib
using System.Text; // strings lib?
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveGameData : MonoBehaviour
{
    string saveFileName = @"C:\Reload_Data\Save_01.txt", saveDir = @"C:\Reload_Data\";
    GameObject EventSystemObject;
    string[] keys = new string[505];
    UI_Interface SettingsInterface;
    [SerializeField] int resolutionIndex, musicVolume, sfxVolume, currentLevel; // data we are storing
    //                      [0 => 8]       [0 => 10]   [0 => 10]
    [SerializeField] int hasPistol = 0, hasRifle = 0;
    [SerializeField] int gameSensitivity; // [20 => 50]
    [SerializeField] int AtBossFightWithCTO; // 0 => 1

    void Awake()
    {
        for (int i = 0; i < 505; i++)
        {
            using (var sha256 = new SHA256Managed())
            {
                keys[i] = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(i.ToString()))).Replace("-", "");
            }
        }
    }

    void Start()
    {
        SettingsInterface = transform.GetComponent<UI_Interface>();
        EventSystemObject = GameObject.Find("EventSystem");

        // print("[*] Checking For Directory. . .");

        if (!Directory.Exists(saveDir)) // check to see if this directory exists, and if not we can made that directory -> we dont wanna constantly remake the directory n lose our data
        {
            // build new directory on the players computer
            // print("[*] Building New Directory. . .");
            Directory.CreateDirectory(saveDir);
        }

        Invoke("CheckForSettingsData", 0.25f);
    }

    void Update()
    {
        hasPistol = PlayerPrefs.GetInt("HasPistol");
        hasRifle = PlayerPrefs.GetInt("HasRifle");
        gameSensitivity = PlayerPrefs.GetInt("Sensitivity");
    }

    void CheckForSettingsData()
    {
        if (File.Exists(saveFileName)) // grab the data if we have a file
        {
            // print("[+] Save File Detected. . .");
            TextReader tr = new StreamReader(saveFileName); // the TextReader reads the txt file and reads the lines to then be converted to game data

            resolutionIndex = DecryptData(tr.ReadLine(), 0); // resolution
            musicVolume = DecryptData(tr.ReadLine(), 0); // music
            sfxVolume = DecryptData(tr.ReadLine(), 0); // sfx
            currentLevel = DecryptData(tr.ReadLine(), 0); // scene saved at

            hasPistol = DecryptData(tr.ReadLine(), 0); // did player have a pistol
            hasRifle = DecryptData(tr.ReadLine(), 0); // did player have a rifle
            
            gameSensitivity = DecryptData(tr.ReadLine(), 200); // game sensitivity

            AtBossFightWithCTO = DecryptData(tr.ReadLine(), 0);

            tr.Close();

            EncryptData();

            if (SettingsInterface != null)
            {
                // print("[*] Importing Data To Game Settings. . .");
                SettingsInterface.Set_SettingsFromFile(resolutionIndex, musicVolume, sfxVolume, gameSensitivity);

                PlayerPrefs.SetInt("HasPistol", hasPistol);
                PlayerPrefs.SetInt("HasRifle", hasRifle);

                if (GameObject.Find("playerBody") != null)
                {
                    if (hasPistol > 0)
                    {
                        if (GameObject.Find("playerBody").GetComponent<Weapons>() != null)
                        {
                            // print("[+] PlayerBody Has Weapons Script");

                            GameObject.Find("playerBody").GetComponent<Weapons>().AquirePistol();
                        }
                    }
    
                    if (hasRifle > 0)
                    {
                        if (GameObject.Find("playerBody").GetComponent<Weapons>() != null)
                        {
                            GameObject.Find("playerBody").GetComponent<Weapons>().AquireRifle();
                        }
                    }
                }
            }

        } else // make a file and set the defaults
            {
                // print("[-] No Save File Detected. . .");
                // print("[*] Constructing New Save File. . .");

                CreateDefaultFile();

                resolutionIndex = 7;
                musicVolume = Convert.ToInt32(0.5f * 10f); // 5
                sfxVolume = Convert.ToInt32(0.5f * 10f); // 5
                gameSensitivity = 200; // 200
                AtBossFightWithCTO = 0;
            }
    }

    void CreateDefaultFile()
    {
        if (!File.Exists(saveFileName))
        {
            // print("[*] Creating File. . .");
        } else // overwrite save
            {
                // print("[*] Overwritting File. . .");
            }

        Invoke("WriteDefaultData", 0.25f);
    }

    void WriteDefaultData()
    {
        // print("[*] Filling In Default Data. . .");

        EncryptData();

        if (SettingsInterface != null)
        {
            // print("[*] Importing Data To Game Settings. . .");
            SettingsInterface.Set_SettingsFromFile(resolutionIndex, musicVolume, sfxVolume, gameSensitivity);
        }
    }

    void WriteCurrentGameData()
    {
        // print("[*] Filling In Current Data. . .");

        EncryptData();
    }

    void EncryptData() // sha256
    {
        // resolution, music, sfx, level

        TextWriter tw = new StreamWriter(saveFileName);

        // tw.WriteLine(resolutionIndex);
        // tw.WriteLine(musicVolume);
        // tw.WriteLine(sfxVolume);

        using (var sha256 = new SHA256Managed()) // taking the game data and writing the encrypted version into the text file
        {
            tw.WriteLine(BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(resolutionIndex.ToString()))).Replace("-", ""));
            tw.WriteLine(BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(musicVolume.ToString()))).Replace("-", ""));
            tw.WriteLine(BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(sfxVolume.ToString()))).Replace("-", ""));
            tw.WriteLine(BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(currentLevel.ToString()))).Replace("-", ""));

            tw.WriteLine(BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(PlayerPrefs.GetInt("HasPistol").ToString()))).Replace("-", ""));
            tw.WriteLine(BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(PlayerPrefs.GetInt("HasRifle").ToString()))).Replace("-", ""));
            
            tw.WriteLine(BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(PlayerPrefs.GetInt("Sensitivity").ToString()))).Replace("-", ""));

            tw.WriteLine(BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(PlayerPrefs.GetInt("InCTO_Fight").ToString()))).Replace("-", ""));
        }

        tw.Close();

        EventSystemObject.SetActive(true);
    }

    int DecryptData(string sha256_data, int starter)
    {
        for (int k = starter; k < 505; k++)
        {
            if (keys[k] == sha256_data)
            {
                return k;
            }
        }

        // print("[-] Token Not Found");
        return 0;
    }

//=====================================================================================================================================================

    public void BuildSaveFile() // accessed via save button UI
    {
        EventSystemObject.SetActive(false);

        PlayerPrefs.SetInt("HasPistol", hasPistol);
        PlayerPrefs.SetInt("HasRifle", hasRifle);

        if (!File.Exists(saveFileName))
        {
            // print("[*] Creating File. . .");
        } else // overwrite save
            {
                // print("[*] Overwritting File. . .");
            }

        resolutionIndex = (int)PlayerPrefs.GetFloat("ResSlideVal");
        musicVolume = Convert.ToInt32(PlayerPrefs.GetFloat("MusicVol") * 10f); // 0-10
        sfxVolume = Convert.ToInt32(PlayerPrefs.GetFloat("SFXVol") * 10f); // 0-10
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        gameSensitivity = PlayerPrefs.GetInt("Sensitivity");
        AtBossFightWithCTO = PlayerPrefs.GetInt("InCTO_Fight");

        WriteCurrentGameData();
    }

    public void LoadGameSave() // accessed via loadSave button UI
    {
        if (File.Exists(saveFileName) && currentLevel > 0)
        {
            // print("[+] Loading Data. . .");

            AudioFades AudioShifter = (AudioFades)FindObjectOfType(typeof(AudioFades));
            
            if (AudioShifter != null)
            {
                AudioShifter.FadeAudioOut();
            }

            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(currentLevel);

            if (SettingsInterface != null)
            {
                SettingsInterface.Set_SettingsFromFile(resolutionIndex, musicVolume, sfxVolume, gameSensitivity);
            }

            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
        } else  
            {
                // print("[-] No Save File Detected Or Valid Level Marker. . .");
            }
    }
}//EndScript