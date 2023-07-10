using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonCellDoors : MonoBehaviour
{
    [SerializeField] bool OpenDoor = false;
    Vector3 closedPos;
    public Transform openPoint;

    [SerializeField] AudioSource cellDoorSound;

    void Awake()
    {
        closedPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (OpenDoor)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPoint.position, 1 * Time.deltaTime);
        } else
            {
                transform.position = Vector3.MoveTowards(transform.position, closedPos, 1 * Time.deltaTime);
            }
    }

    public void OpenCellDoors()
    {
        if (cellDoorSound != null)
        {
            cellDoorSound.Play();
        }

        OpenDoor = true;
    }

    public void CloseCellDoors()
    {
        if (cellDoorSound != null)
        {
            cellDoorSound.Play();
        }
        
        OpenDoor = false;
    }
    
}//EndScript