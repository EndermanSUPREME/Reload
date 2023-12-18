using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prisonCCTV_Camera : MonoBehaviour
{
    Transform PlayerHead, cameraAlertBar;
    public Transform camRaycastPoint;
    public float viewRadius, viewAngle;
    [Range(-1.75f, 1.75f)] public float fillLength;
    public LayerMask playerMask;
    public Renderer CameraLens;
    public Material NormalLens, WatchingLens;
    LevelWideAlertness BotNetAlert;
    PrisonAlarm prisonAlarm;
    [SerializeField] bool podCamera, nightTime = false, playerInCell,
                    cameraCheck = false, triggeredAlarm = false, watchingPlayer = false;

    void Start()
    {
        GameObject playerInScene = GameObject.Find("PlayerCam");
        PlayerHead = playerInScene.transform;

        GameObject camBarObj = GameObject.Find("CameraDetectionBar");
        cameraAlertBar = camBarObj.transform;

        GameObject botnetScripObj = GameObject.Find("BotNetAlert");
        BotNetAlert = botnetScripObj.GetComponent<LevelWideAlertness>();

        GameObject prisonAlarmObj = GameObject.Find("PrisonAlert");
        prisonAlarm = prisonAlarmObj.GetComponent<PrisonAlarm>();

        cameraAlertBar.localScale = new Vector3(1, 0, 1);
    }

    void FixedUpdate()
    {
        CameraVision();
    }

    // If all the camera arent detecting the player then we will reset the meter for the cameras
    public bool isWatchingPlayer()
    {
        return watchingPlayer;
    }

    void CameraVision()
    {
        playerInCell = GameObject.Find("PlayerCellTrigger").GetComponent<PrisonSchedule>().isPlayerInTheirCell();
        camRaycastPoint.position = new Vector3(camRaycastPoint.position.x, PlayerHead.position.y, camRaycastPoint.position.z);
        
        Collider[] objectsNearby = Physics.OverlapSphere(camRaycastPoint.position, viewRadius, playerMask);

        for (int i = 0; i < objectsNearby.Length; i++)
        {
            Vector3 DirToTarget = (objectsNearby[i].transform.position - camRaycastPoint.position).normalized;
            Debug.DrawRay(camRaycastPoint.position, DirToTarget, Color.green);

            if (Vector3.Angle(camRaycastPoint.forward, DirToTarget) <= viewAngle / 2 && !triggeredAlarm)
            {
                RaycastHit hit;
                if (Physics.Raycast(camRaycastPoint.position, DirToTarget, out hit, playerMask))
                {
                    if (hit.transform == PlayerHead)
                    {
                        // detected
                        CameraLens.material = WatchingLens;

                        if (PlayerHead.gameObject.layer == 7 && !GameObject.Find("prisonCardReader").GetComponent<PlayerEscapePrisonPod>().isPlayerDisguised())
                        {
                            prisonAlarm.SecurityAlert();
                            triggeredAlarm = true;
                        }

                        if (cameraAlertBar != null)
                        {
                            if (!podCamera)
                            {
                                DisplayCameraAwareness();
                            }

                            if (podCamera && !playerInCell && cameraCheck)
                            {
                                if (nightTime && !GameObject.Find("prisonCardReader").GetComponent<PlayerEscapePrisonPod>().isPlayerDisguised() || !GameObject.Find("prisonCardReader").GetComponent<PlayerEscapePrisonPod>().isPlayerDisguised() && PlayerHead.gameObject.layer == 7)
                                {
                                    fillLength = 1.5f;
                                }
                            }
    
                            if (fillLength >= 1)
                            {
                                BotNetAlert.AlertAllBots();
                                prisonAlarm.SecurityAlert();
                                triggeredAlarm = true;
                                Destroy(cameraAlertBar.parent.gameObject);
                            }
                        }
                    } else
                        {
                            watchingPlayer = true;
                            CameraLens.material = NormalLens;
                            fillLength = 0;
                        }
                }
            } else
                {
                    CameraLens.material = NormalLens;
                    fillLength = 0;
                }
        }

        // print(PlayerHead.localRotation.x * 10 + " : " + (((PlayerHead.localRotation.x * 10) - 1.75f) / (-1.75f * 2f)));
    }

    void DisplayCameraAwareness()
    {
        fillLength = (((PlayerHead.localRotation.x * 10) - 1.75f) / (-1.75f * 2f));

        if (fillLength >= 0 && fillLength <= 1)
        {
            cameraAlertBar.localScale = new Vector3(1, fillLength, 1);

            watchingPlayer = true;
        } else if (fillLength < 0)
            {
                cameraAlertBar.localScale = new Vector3(1, 0, 1);

                watchingPlayer = false;
            } else if (fillLength > 1)
                {
                    cameraAlertBar.localScale = new Vector3(1, 1, 1);

                    watchingPlayer = true;
                }

        // Debug.Log("[+] Camera can see the player :: " + watchingPlayer.ToString());
    }

    public void ResetCameraDetectionMeter()
    {
        fillLength = 0;
        cameraAlertBar.localScale = new Vector3(1, 0, 1);

        // Debug.Log("[*] Resetting Camera Bar");
    }

    public void ActivateNightCamera()
    {
        cameraCheck = true;
    }

    public void DeactivateNightCamera()
    {
        cameraCheck = false;
    }

    public void TimeOfDay()
    {
        if (nightTime)
        {
            nightTime = false;
        } else
            {
                nightTime = true;
            }
    }

    public void SetCameraToDefault()
    {
        podCamera = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Debug.DrawRay(camRaycastPoint.position, camRaycastPoint.forward, Color.yellow);
    }

}//EndScript