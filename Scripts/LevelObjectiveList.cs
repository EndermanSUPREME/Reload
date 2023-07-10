using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjectiveList : MonoBehaviour
{
    [SerializeField] string[] objectiveList;
    int index = 0;

    public string GetCurrentObjective()
    {
        if (index < objectiveList.Length)
        {
            return objectiveList[index];
        } else
            {
                return "All Objectives Completed!";
            }
    }

    public void ObjectiveCompleted()
    {
        index++;
    }

    public void ResetPrisonObjective()
    {
        index = 0;
    }
}//EndScript