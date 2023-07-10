using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveItemPickup : MonoBehaviour
{
    public void PickUpItem()
    {
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        gameObject.SetActive(false);
    }

    public void PickUpGear()
    {
        gameObject.SetActive(false);
    }
}//EndScript