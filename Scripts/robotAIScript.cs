using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class robotAIScript : MonoBehaviour
{
    [SerializeField] bool Hostile = false, runAimlessly;
    [SerializeField] float viewRadiusSet, viewAngle, speedConst;
    float speed, fillLength;

    public LayerMask playerLayer, DeadBody;

    [SerializeField] Transform Eye, awarenessBar;

    Vector3 directionToTarget;

    bool alerted = false, playerFound = false, playerInDisguise = false,
        idle = false, OnGuard = false, fireAtPlayer = false, shotFired = false,
        fleeing = false, goingToAltPos = false, canSeePlayer = false, noticesPlayer = false;

    Animator robotAnim;
    Transform altPos;

    [SerializeField] Transform[] destinations;

    UI_Interface PlayerUI;
    LevelWideAlertness BotNetAlert;

    NavMeshAgent agent;
    HealthScript healthScript;

    [SerializeField] string civilianBotAlarmAnimName;

    bool forceSetDestination = false;

/*================================ USED BY HOSTILE ROBOTS ==============================================================*/
    [SerializeField] Transform robotSpine, GunHand, Player, PlayerCamera; // camera with sphere collider
    [SerializeField] GameObject bulletPref;
    [SerializeField] AudioSource robot_gunShot;

    void Awake()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
        {
            transform.position = hit.point;
        }

        for (int i = 0; i < destinations.Length; i++)
        {
            if (Physics.Raycast(destinations[i].position, -destinations[i].up, out RaycastHit hit2))
            {
                destinations[i].position = hit2.point + new Vector3(0, 0.1f, 0);
            }
        }
    }

    void Start()
    {
        robotAnim = transform.GetComponent<Animator>();
        healthScript = transform.GetComponent<HealthScript>();

        agent = transform.GetComponent<NavMeshAgent>();

        GameObject botnetScripObj = GameObject.Find("BotNetAlert");
        BotNetAlert = botnetScripObj.GetComponent<LevelWideAlertness>();

        if (GameObject.Find("RobotSuspisionBar") != null)
        {
            awarenessBar = GameObject.Find("RobotSuspisionBar").transform;
        }

        if (agent.enabled) agent.SetDestination(destinations[Random.Range(0, destinations.Length)].position);

        speed = speedConst;
        agent.speed = speedConst;
    }

    public void FindDetectionBarFromSaveLoad()
    {
        if (GameObject.Find("RobotSuspisionBar") != null)
        {
            awarenessBar = GameObject.Find("RobotSuspisionBar").transform;
        }
    }

    void Update()
    {
        if (GameObject.Find("LevelObject") == null)
        {
            agent.enabled = false;
            return;
        } else
            {
                agent.enabled = true;
            }

        if (GameObject.Find("playerBody") != null)
        {
            GameObject playerCamInScene = GameObject.Find("PlayerCam");
            PlayerCamera = playerCamInScene.transform;

            GameObject playerInScene = GameObject.Find("playerBody");
            Player = playerInScene.transform;

            PlayerUI = playerCamInScene.GetComponent<UI_Interface>();

            if (GameObject.Find("RobotSuspisionBar") != null)
            {
                awarenessBar = GameObject.Find("RobotSuspisionBar").transform;
            }
        }

        if (forceSetDestination)
        {
            ForceDestination();
        }

        if (!alerted)
        {
            if (!idle && !goingToAltPos)
            {
                Roam();
            }

            if (goingToAltPos && altPos != null)
            {
                if (agent.enabled) agent.SetDestination(altPos.position);
            }
        } else
            {
                if (Player != null)
                {
                    if (Hostile)
                    {
                        if (playerFound)
                        {
                            playerInDisguise = false;
                            BotNetAlert.AlertAllBots();
                        }
                    } else
                        {
                            if (runAimlessly)
                            {
                                Flee();
                            } else
                                {
                                    Cower();
                                }
                        }
                }
            }

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (Hostile)
        {
            FindingTargetsInsideView();
        }

        FindDeadBodiesInView();
    }

    void LateUpdate() // rotate arms to shoot at player
    {
        if (Hostile && playerFound)
        {
            AttackPlayer();
        }
    }

//====================================================================================================//

    void AttackPlayer()
    {
        StopCoroutine(IdleDelay(0));

        Vector3 EndPt = Player.position;
        Vector3 AIPos = transform.position;

        if (canSeePlayer)
        {
            if (Vector3.Distance(AIPos, EndPt) <= 7.25f && !fireAtPlayer)
            {
                fireAtPlayer = true;
            }   
                
                if (fireAtPlayer) // shooting at player
                {
                    if (Vector3.Distance(AIPos, EndPt) > 11.25f)
                    {
                        fireAtPlayer = false;
                    } else
                        {
                            agent.speed = 0;

                            robotAnim.Play("shoot");

                            // body spins to look at player
                            transform.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));

                            // body bends to aim vertically at player camera that holds a collider
                            robotSpine.LookAt(new Vector3(PlayerCamera.position.x, PlayerCamera.position.y - 0.15f, PlayerCamera.position.z));

                            if (!shotFired)
                            {
                                StartCoroutine(EnemyRateOfFire());
                            }
                        }
                }   
                
                    if (Vector3.Distance(AIPos, EndPt) > 7f && !fireAtPlayer) // player gets far away while enemy shooting
                    {
                        StopCoroutine(EnemyRateOfFire());

                        robotAnim.Play("movement");

                        AdjustSpeed();
                        if (agent.enabled) agent.SetDestination(new Vector3(Player.position.x, Player.position.y - 0.5f, Player.position.z));
                    }
        } else // no longer seen
            {
                StopCoroutine(EnemyRateOfFire());
                
                AdjustSpeed();
                if (agent.enabled) agent.SetDestination(new Vector3(Player.position.x, Player.position.y - 0.5f, Player.position.z));

                fireAtPlayer = false;
            }
    }

    IEnumerator EnemyRateOfFire()
    {
        shotFired = true;

        GunHand.GetComponent<ParticleSystem>().Play();
        robot_gunShot.Play();
        GunHand.LookAt(new Vector3(PlayerCamera.position.x, PlayerCamera.position.y, PlayerCamera.position.z));
        GameObject newBullet = Instantiate(bulletPref, GunHand.position, GunHand.rotation);

        yield return new WaitForSeconds(0.5f);
        shotFired = false;
    }

    void FindingTargetsInsideView()
    {
        Collider[] targetsNearby = Physics.OverlapSphere(Eye.position, viewRadiusSet, playerLayer);

        for (int i = 0; i < targetsNearby.Length; i++)
        {
            Transform target = targetsNearby[i].transform;

            directionToTarget = (target.position - Eye.position).normalized; // scales from 0-1

            // takes an angle between a ray that shoots straight forward from its position and a ray that points from itself to the target checking if its inside the bounded view angle
            if (Vector3.Angle(Eye.forward, directionToTarget) < viewAngle/2)
            {
                // print("Ray Hit Player");

                RaycastHit enemyEyeRay;
                if (Physics.Raycast(Eye.position, directionToTarget, out enemyEyeRay, viewRadiusSet))
                {
                    // print(enemyEyeRay.transform.name + " : " + enemyEyeRay.transform.gameObject.layer);

                    if (enemyEyeRay.transform.gameObject.layer == 7 || enemyEyeRay.transform.gameObject.layer == 11)
                    {
                        // if the player is seen
                        canSeePlayer = true;

                        if (!playerInDisguise && awarenessBar == null)
                        {
                            PlayerUI.PlayerSpotted();
                            healthScript.Alerted();
    
                            alerted = true;
                            playerFound = true;

                            playerInDisguise = false;
                        } else
                            {
                                if (awarenessBar != null)
                                {
                                    float distFromPlayer = Vector3.Distance(transform.position, target.position) - 1.5f;
    
                                    if (distFromPlayer >= 1 && distFromPlayer <= 8)
                                    {
                                        fillLength = ((distFromPlayer - 8f) / -8f);
                                        awarenessBar.localScale = new Vector3(1, fillLength, 1);

                                        noticesPlayer = true;
                                    } else if (distFromPlayer < 1)
                                        {
                                            fillLength = 1.5f;
                                            awarenessBar.localScale = new Vector3(1, 1, 1);
                                            AttackedByPlayer();
                                        } else if (distFromPlayer > 8)
                                            {
                                                fillLength = 0;
                                                awarenessBar.localScale = new Vector3(1, 0, 1);
                                                noticesPlayer = false;
                                            }
                                }
                            }
                    } else
                        {
                           canSeePlayer = false;
                           noticesPlayer = false;
                        }
                }
            } else
                {
                    canSeePlayer = false;
                    noticesPlayer = false;
                }
        }
    }

    public void PlayerWearingDisguise()
    {
        playerInDisguise = true;
    }

    public bool RobotSeesPlayer()
    {
        return noticesPlayer;
    }

    public void ResetRobotDetectionMeter()
    {
        fillLength = 0;
        awarenessBar.localScale = new Vector3(1, 0, 1);

        // Debug.Log("[*] Resetting Bar");
    }

    void FindDeadBodiesInView()
    {
        Collider[] bodiesNearby = Physics.OverlapSphere(Eye.position, viewRadiusSet, DeadBody);

        for (int i = 0; i < bodiesNearby.Length; i++)
        {
            Transform target = bodiesNearby[i].transform;

            directionToTarget = (target.position - Eye.position).normalized; // scales from 0-1

            // takes an angle between a ray that shoots straight forward from its position and a ray that points from itself to the target checking if its inside the bounded view angle
            if (Vector3.Angle(Eye.forward, directionToTarget) < viewAngle/2)
            {
                RaycastHit enemyEyeRay;
                if (Physics.Raycast(Eye.position, directionToTarget, out enemyEyeRay, viewRadiusSet))
                {
                    print("Body Found : " + enemyEyeRay.transform.gameObject.layer);

                    if (enemyEyeRay.transform.gameObject.layer == 8)
                    {
                        // if a dead body is found
                        PlayerUI.BodyFound();
                        BotNetAlert.AlertForBodyFound();
                        RobotBodyFound();
                    }
                }
            }
        }
    }

//====================================================================================================//

    private void ForceDestination()
    {
        if (agent.enabled) agent.SetDestination(destinations[Random.Range(0, destinations.Length)].position);
        forceSetDestination = false;
    }

    void Roam()
    {
        Vector3 EndPt = new Vector3(agent.destination.x, 0, agent.destination.z);
        Vector3 AIPos = new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(AIPos, EndPt) < 0.05f)
        {
            idle = true;

            if (!goingToAltPos)
            {
                StartCoroutine(IdleDelay(Random.Range(2,4)));
            }
        }
    }

    public void FindPlayerInShower(Transform sentPos)
    {
        goingToAltPos = true;
        
        StopCoroutine(IdleDelay(0));

        idle = false;
        agent.speed = speedConst;

        altPos = sentPos;
        if (agent.enabled) agent.SetDestination(sentPos.position);
    }

    public void GoToCell(Vector3 PrisonCellPos)
    {
        goingToAltPos = true;
        StopCoroutine(IdleDelay(0));
        if (agent.enabled) agent.SetDestination(PrisonCellPos);
    }

    public void LeaveCell()
    {
        StartCoroutine(IdleDelay(2));
        goingToAltPos = false;
    }

    IEnumerator IdleDelay(int sec)
    {
        yield return new WaitForSeconds(sec);
        agent.speed = 0;
        idle = false;

        if (agent.enabled)
        {
            if (!goingToAltPos)
            {
                if (agent.enabled) agent.SetDestination(destinations[Random.Range(0, destinations.Length)].position);
                agent.speed = speedConst;
            }
        }
    }

//====================================================================================================//

    void Cower()
    {
        if (!fleeing)
        {
            if (agent.enabled)
            {
                agent.SetDestination(destinations[Random.Range(0, destinations.Length)].position);
            }

            fleeing = true;
        }

        Vector3 EndPt = new Vector3(agent.destination.x, 0, agent.destination.z);
        Vector3 AIPos = new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(AIPos, EndPt) < 0.05f)
        {
            agent.speed = 0;
            agent.enabled = false;
            robotAnim.Play(civilianBotAlarmAnimName);
        }
    }

    void Flee()
    {
        robotAnim.Play(civilianBotAlarmAnimName);

        Vector3 EndPt = new Vector3(agent.destination.x, 0, agent.destination.z);
        Vector3 AIPos = new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(AIPos, EndPt) < 0.05f)
        {
            if (agent.enabled) agent.SetDestination(destinations[Random.Range(0, destinations.Length)].position);
        }
    }

    public void RobotBodyFound()
    {
        if (!OnGuard)
        {
            // will not chase the player but removes the bonus of a sneak attack
            healthScript.Alerted();
            OnGuard = true;
            // becomes more aware of its surroundings
            viewRadiusSet += 5;
            viewAngle += 10;
        }

        alerted = true;
    }

    public void AttackedByPlayer()
    {
        if (Hostile && PlayerUI != null)
        {
            PlayerUI.PlayerSpotted();
            playerFound = true;
        }

        healthScript.Alerted();
        
        alerted = true;
    }

//====================================================================================================//

    void AdjustSpeed()
    {
        Vector3 EndPt = new Vector3(agent.destination.x, 0, agent.destination.z);
        Vector3 AIPos = new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(AIPos, EndPt) > 10)
        {
            if (speed < speedConst * 1.95f)
            {
                speed += 0.015f;
            } else
                {
                    speed = speedConst * 1.95f;
                }
        } else
            {
                if (speed > speedConst)
                    {
                        speed -= 0.015f;
                    } else
                        {
                            speed = speedConst;
                        }
            }

        agent.speed = speed;
    }

    void UpdateAnimator()
    {
        Vector3 vel = agent.velocity;
        Vector3 localVel = transform.InverseTransformDirection(vel); // Transforms the direction from world space to local space
        float speed = localVel.z;

        robotAnim.SetFloat("forwardSpeed", speed);
    }
    
//====================================================================================================//

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(Eye.position, viewRadiusSet);

        for (int i = 0; i < destinations.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(destinations[i].position, 0.1f);
        }

        // Vector3 AngleA = new Vector3(Mathf.Sin(viewAngle * Mathf.Deg2Rad), 0, Mathf.Cos(viewAngle * Mathf.Deg2Rad));
        // Vector3 AngleB = new Vector3(Mathf.Sin(-viewAngle * Mathf.Deg2Rad), 0, Mathf.Cos(-viewAngle * Mathf.Deg2Rad));

        // Gizmos.color = Color.green;
        // Gizmos.DrawLine(Eye.position, AngleA * -viewRadiusSet);
        // Gizmos.DrawLine(Eye.position, AngleB * -viewRadiusSet);

        // Gizmos.color = Color.yellow;
        // Gizmos.DrawLine(Eye.position, Eye.forward);

        // if (GunHand != null)
        // {
        //     Debug.DrawRay(GunHand.position, GunHand.forward, Color.blue);
        // }
    }

}//EndScript