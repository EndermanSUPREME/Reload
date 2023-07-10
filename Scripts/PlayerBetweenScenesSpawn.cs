using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBetweenScenesSpawn : MonoBehaviour
{
    bool rePositionedPlayer = false;
    public GameObject PlayerImportPref;
    public Material gaurdMat;
    public robotAIScript[] securityBots;
    bool changePlayerColor = false;

    public int pistolMag, pistolTotal, rifleMag, rifleTotal;

    void Start()
    {
        if (GameObject.Find("playerBody") == null) // when scene loads from a save file
        {
            GameObject newPlayer = Instantiate(PlayerImportPref, transform.position, PlayerImportPref.transform.rotation);
            newPlayer.transform.name = "playerBody";

            newPlayer.GetComponent<movement>().enabled = false;

            Invoke("AdjustFromSaveLoad", 1f);

            Invoke("PositionPlayerOnLoad", 0.525f);
        } else // player transfered to level OR Found after player Dies
            {
                if (!GameObject.Find("PlayerCam").GetComponent<HealthScript>().isThisPlayerDead()) // if the player loaded isnt dead resume like normal
                {
                    if (SceneManager.GetActiveScene().buildIndex == 11) // runs after you leave prisonExterior
                    {
                        GameObject.Find("PlayerCam").transform.GetComponent<Weapons>().TryGiftGearFromTransfer();
                    }

                    GameObject.Find("playerBody").transform.position = transform.position;

                    GameObject.Find("playerBody").GetComponent<movement>().enabled = false;

                    GameObject.Find("playerBody").transform.position = transform.position;

                    Invoke("PositionPlayerOnLoad", 0.5f);
                } else // if it finds a dead player
                    {
                        Destroy(GameObject.Find("playerBody"));
                        Invoke("Start", 0.05f);
                    }
            }
    }

    void Update()
    {
        if (changePlayerColor)
        {
            GameObject[] bodyParts = GameObject.FindGameObjectsWithTag("bodyParts");

            if (gaurdMat != null && bodyParts != null)
            {
                foreach (GameObject part in bodyParts)
                {
                    part.transform.GetComponent<Renderer>().material = gaurdMat;
                }
            }

            if (bodyParts != null && bodyParts.Length > 0)
            {
                if (bodyParts[2].transform.GetComponent<Renderer>().material == gaurdMat)
                {
                    changePlayerColor = false;
                }
            }
        }
    }

    void PositionPlayerOnLoad()
    {
        if (!rePositionedPlayer && GameObject.Find("playerBody") != null)
        {
            GameObject.Find("playerBody").transform.position = transform.position;

            GameObject.Find("PlayerCam").transform.GetComponent<Weapons>().SetLevelAmmoDefault(pistolMag, pistolTotal, rifleMag, rifleTotal);

            GameObject.Find("playerBody").GetComponent<movement>().enabled = true;
            rePositionedPlayer = true;
        }
    }

    void AdjustFromSaveLoad()
    {
        if (SceneManager.GetActiveScene().buildIndex > 8) // outside prison and after => loading save file
        {
            GameObject.Find("PlayerCam").transform.GetComponent<Weapons>().GiveGearFromSave();
        }

        if (SceneManager.GetActiveScene().buildIndex == 9) // outside prison
        {
            changePlayerColor = true;

            foreach (robotAIScript bot in securityBots)
            {
                bot.PlayerWearingDisguise();
                bot.FindDetectionBarFromSaveLoad();
            }
        }
    }
}//EndScript