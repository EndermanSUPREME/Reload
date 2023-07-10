using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOne_Reforcements : MonoBehaviour
{
    public LevelWideAlertness botnet;
    [SerializeField] Transform doorPivot, doorOpenPos;
    bool soundPlayed = false;
    [SerializeField] AudioSource doorSound;

    void Update()
    {
        if (botnet.isAreaOnAlert())
        {
            doorPivot.position = Vector3.MoveTowards(doorPivot.position, doorOpenPos.position, 2 * Time.deltaTime);

            if (!soundPlayed)
            {
                doorSound.Play();
                soundPlayed = true;
            }
        }

        if (doorPivot.position == doorOpenPos.position)
        {
            this.enabled = false;
        }
    }
}//EndScript