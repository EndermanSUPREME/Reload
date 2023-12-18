using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonSchedule : MonoBehaviour
{
    public int RoamTimeMinutes;
    [SerializeField] robotAIScript[] Prisoners;
    Vector3[] cellPositions;
    [SerializeField] PrisonCellDoors[] CellDoors;
    public prisonCCTV_Camera PodCamera;
    public Pod_BotScript podGaurd;
    public BoxCollider playerBedCollider;
    public bool timeIsDay = true, playerInCell;

    public AudioSource lightsOutAnnouncement;

    void Awake()
    {
        cellPositions = new Vector3[Prisoners.Length];

        for (int i = 0; i < Prisoners.Length; i++)
        {
            cellPositions[i] = Prisoners[i].transform.position;
        }
    }

    void Start()
    {
        int routineSec = RoamTimeMinutes * 60;

        StartCoroutine(PrisonDayLight(routineSec));
    }

    public void BeginNewPodDay()
    {
        PodCamera.TimeOfDay();

        PodCamera.DeactivateNightCamera();

        playerBedCollider.enabled = true;

        timeIsDay = true;
        
        cellPositions = new Vector3[Prisoners.Length];

        for (int i = 0; i < Prisoners.Length; i++)
        {
            cellPositions[i] = Prisoners[i].transform.position;
        }

        int routineSec = RoamTimeMinutes * 60;

        StartCoroutine(PrisonDayLight(routineSec));
    }

    IEnumerator PrisonDayLight(int sec)
    {
        foreach (PrisonCellDoors cellDoor in CellDoors)
        {
            cellDoor.OpenCellDoors();
        }

        foreach (robotAIScript prisoner in Prisoners)
        {
            prisoner.LeaveCell();
        }

        yield return new WaitForSeconds(sec);

        if (PodCamera.transform.gameObject.activeSelf)
        {
            LightsOut();
        } else
        {
            StartCoroutine(LevelCheck());
        }
    }

    void LightsOut()
    {
        timeIsDay = false;

        PodCamera.TimeOfDay();

        playerBedCollider.enabled = false;

        lightsOutAnnouncement.Play();

        for (int k = 0; k < 10;)
        {
            for (int i = 0; i < Prisoners.Length; i++)
            {
                Prisoners[i].GoToCell(cellPositions[i]);
            }

            k++;
        }

        Invoke("CheckIfPlayerIsInCell", 30);
    }

    IEnumerator LevelCheck()
    {
        yield return new WaitForSeconds(1);

        if (PodCamera.transform.gameObject.activeSelf)
        {
            LightsOut();
        } else
        {
            StartCoroutine(LevelCheck());
        }
    }

    public bool GetTimeOfDay()
    {
        return timeIsDay;
    }

    void CheckIfPlayerIsInCell()
    {
        foreach (PrisonCellDoors cellDoor in CellDoors)
        {
            cellDoor.CloseCellDoors();
        }

        Invoke("CameraAlertness", 3);
    }

    void CameraAlertness()
    {
        PodCamera.ActivateNightCamera();

        podGaurd.GaurdSearchForPlayer();
    }

    public bool isPlayerInTheirCell()
    {
        return playerInCell;
    }

    void OnTriggerStay(Collider collider)
    {
        playerInCell = true;
    }

    void OnTriggerExit(Collider collider)
    {
        playerInCell = false;
    }

}//EndScript