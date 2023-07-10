using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneSecutiryDoor : MonoBehaviour
{
    [SerializeField] Transform[] monitorScreens;
    public Material interactionMaterial;
    [SerializeField] bool unlocked = false;
    [SerializeField] Transform slidingDoorPivot, DoorOpenPoint;
    public Transform interactionPref;

    void Start()
    {
        int chosenScreen = Random.Range(0, monitorScreens.Length);

        monitorScreens[chosenScreen].GetComponent<Renderer>().material = interactionMaterial;
        
        interactionPref.parent = monitorScreens[chosenScreen];
        interactionPref.localPosition = Vector3.zero;
        interactionPref.localRotation = new Quaternion(0, 0, 0, 0);
    }

    public void MonitorInteraction()
    {
        unlocked = true;
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
    }

    void FixedUpdate()
    {
        if (unlocked)
        {
            slidingDoorPivot.position = Vector3.MoveTowards(slidingDoorPivot.position, DoorOpenPoint.position, 4 * Time.deltaTime);

            if (slidingDoorPivot.position == DoorOpenPoint.position)
            {
                this.enabled = false;
            }
        }
    }
}//EndScript