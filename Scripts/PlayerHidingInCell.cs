using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHidingInCell : MonoBehaviour
{
    public PrisonSchedule prisonSchedule;
    public PrisonGoToNextDay sleepInPrisonScript;

    public Transform playerBody, playerCellPosition;
    bool warpPlayer = false;

    void GetPlayerBackIntoThierCell()
    {
        sleepInPrisonScript.SleepInCell(); // fade into black
        Invoke("TeleportPlayer", 2.1f);
    }

    void TeleportPlayer()
    {
        playerBody.position = playerCellPosition.position;
        warpPlayer = false;
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.name == "PlayerCam" && !prisonSchedule.GetTimeOfDay()) // if the player is trapped in another cell at night
        {
            // teleport player back to cell
            if (!warpPlayer)
            {
                Invoke("GetPlayerBackIntoThierCell", 32.1f);
                warpPlayer = true;
            }
        }
    }
}//EndScript