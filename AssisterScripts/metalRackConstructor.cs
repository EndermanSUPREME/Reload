using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class metalRackConstructor : MonoBehaviour
{
    public GameObject[] boxesOnRack, panels, metalRacksInScene;
    public bool AutoBuild = false;

    void FillObjects()
    {
        for (int i = 0; i < metalRacksInScene.Length;)
            {
                for (int j = 0; j < panels.Length;)
                {
                    GameObject newPanel = Instantiate(panels[j]);
                    newPanel.transform.parent = metalRacksInScene[j].transform;
                    newPanel.transform.localPosition = metalRacksInScene[j].transform.localPosition;
    
                    j++;
                }
    
                int RandomStart = Random.Range(0, 1);
                
                int RandomEnd = boxesOnRack.Length - Random.Range(0, 1);
                
                for (int k = RandomStart; k < RandomEnd;)
                {
                    GameObject newBox = Instantiate(boxesOnRack[k]);
                    newBox.transform.parent = metalRacksInScene[k].transform;
                    newBox.transform.localPosition = metalRacksInScene[k].transform.localPosition;
    
                    k++;
                }
                i++;
            }
    }

    void OnDrawGizmos()
    {
        if (AutoBuild)
        {
            // FillObjects(); // Function Causes Unlimited Objects to Instantiate, needs fixed if decided to progress on the script
            AutoBuild = false;
        }
    }
}//EndScript