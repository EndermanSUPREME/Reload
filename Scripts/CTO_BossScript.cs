using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CTO_BossScript : MonoBehaviour
{
    HealthScript healthScript;
    [SerializeField] DisableSecurityAndEliminateTheCTO LevelObjectiveScript; // obnjective script for level
    bool activate = false, canSeePlayer, shoot = true, moveToJumpPoint = false, jumpPointSet = false, jumping = false, jumpScan = false, jumpingUp;
    bool missleLauncherReady = false, shotObstructed = false;
    public int showHealth;
    int maxHealth;
    [SerializeField] Animator CTO_Anim, MissleLauncherAnim;
    NavMeshAgent agent;
    Transform Player, playerHead;
    [SerializeField] Transform CTO_Head, leftUpperArm, leftLowerArm, middleSpine, upperSpine, bulletSpawn, CTO_HealthBar, missileSpawn;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject bulletPref, CTO_HealthUI, firedMissile, MissilePref;
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] float SpeedConst, minDist, missleGeneralSpeed;
    float speed, distanceFromPlayer;
    [SerializeField] CTO_JumpPointScript[] jumpPaths; // contains vectors that build a jump path

    Vector3 playerPos, jumpPos;
    int chosenIndex, ind, pathIndex = 0;

    [SerializeField] AudioSource CTO_gunShot, missileLauncher_Sound;

    void Start()
    {
        healthScript = GetComponent<HealthScript>();
        Player = GameObject.Find("playerBody").transform;
        playerHead = GameObject.Find("PlayerCam").transform;

        agent = GetComponent<NavMeshAgent>();
        CTO_Anim = GetComponent<Animator>();
        MissleLauncherAnim.enabled = false;

        maxHealth = healthScript.GetHealth();

        CTO_HealthUI.transform.SetParent(GameObject.Find("HUD").transform); // move the HUD from the object to the player when the CTO gets activated

        CTO_Activate(); // allows update to run code blocks
    }

    void Update()
    {
        if (activate)
        {
            PlayerPrefs.SetInt("InCTO_Fight", 1);
            
            Vector3 PlayerPosition = new Vector3(Player.position.x, transform.position.y, Player.position.z);
            distanceFromPlayer = Vector3.Distance(transform.position, PlayerPosition);

            if (CTO_HealthUI != null)
            {
                CTO_HealthUI.GetComponent<RectTransform>().localPosition = Vector3.zero;
                CTO_HealthUI.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
                
                CTO_HealthUI.SetActive(true);
            }
            
            CTO_Vision();
            Movement();

            Death();

            showHealth = healthScript.GetHealth(); // display health on inspector
        } else
            {
                if (jumpingUp)
                {
                    FollowJumpUp_Path(); // jump to a higher floor
                } else
                    {
                        FollowJumpDown_Path(); // jump to a lower floor
                    }
            }
    }

    void FixedUpdate() // missles use rigidBodies
    {
        if (missleLauncherReady) MissleLauncherAttack();
    }

    void LateUpdate() // allows rig manipulation via code
    {
        if (!jumping)
        {
            if (distanceFromPlayer > 4 && CTO_Anim.GetBool("shootAtPlayer")) // when at a distance we want to shoot the player
            {
                AimArmAtPlayer();
            }
        }
    }

    public void CTO_Activate()
    {
        activate = true;
    }

    public void MissileShot_Obstructed() // external trigger objects
    {
        shotObstructed = true;
    }

    public void MissileShot_Clear() // external trigger objects
    {
        shotObstructed = false;
    }

//=======================================================================================
//==================================  MOVEMENT  =========================================
//=======================================================================================

    void CTO_Vision()
    {
        Vector3 Dir = (CTO_Head.position - playerHead.position).normalized;

        RaycastHit hit;
        if (Physics.Raycast(CTO_Head.position, Dir, out hit, playerLayerMask))
        {
            // cto can see player clearly
            canSeePlayer = true;
        } else
            {
                canSeePlayer = false;
            }
    }

    void Movement()
    {
        playerPos = new Vector3(Player.position.x, Player.position.y + 0.55f, Player.position.z); // keeps the agent.destination off the floor incase Unity wants to say its unreachable

        if (!moveToJumpPoint) // when on the same floor chase the player
        {
            if (agent.enabled && speed > 0)
            {
                agent.SetDestination(playerPos);
            }

            if (agent.enabled && speed == 0)
            {
                agent.SetDestination(transform.position);
            }

        } else // when the player tries to get high/low ground follow
            {
                if (!jumping)
                {
                    if (!jumpPointSet)
                    {
                        if (!jumpScan)
                        {
                            if (jumpingUp)
                            {
                                FindClosestJumpUp_Point();
                            } else
                                {
                                    FindClosestJumpDown_Point();
                                }
                        }
                    } else
                        {
                            // print("[*] Agent Set To => " + jumpPos);

                            agent.SetDestination(jumpPos);

                            SpeedAdjust(jumpPos);

                            if (transform.position == new Vector3(jumpPos.x, transform.position.y, jumpPos.z))
                            {
                                speed = 0;
                                JumpUpToDifferentGround();

                                jumping = true;
                            }
                        }
                }
            }

        if (agent.enabled && !moveToJumpPoint)
        {
            NavMeshPath navMeshPath = new NavMeshPath();
    
            if (agent.CalculatePath(playerPos, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                // if the destination is reachable

                if (distanceFromPlayer > minDist)
                {
                    // run towards destination
                    if (speed < SpeedConst * 2.25f)
                    {
                        speed += 0.05f;
                    } else
                        {
                            speed = SpeedConst * 2.25f;
                        }
                } else
                    {
                        // walk towards destination
                        if (speed > SpeedConst)
                        {
                            speed -= 0.05f;
                        } else
                            {
                                speed = SpeedConst;
                            }
                    }
            } else
                {
                    // if the destination cannot be reached

                    if (!jumpPointSet)
                    {
                        if (Player.position.y - transform.position.y >= 5)
                        {
                            // jump to upper floor
                            jumpingUp = true;
                            
                            SpeedAdjust(jumpPos);
    
                            moveToJumpPoint = true;
                        } else if (Player.position.y - transform.position.y < -6)
                            {
                                // jump to lower floor
                                jumpingUp = false;
    
                                SpeedAdjust(jumpPos);
    
                                moveToJumpPoint = true;
                            } else if (Player.position.y - transform.position.y < 5) // the elevation difference is not significant
                                {
                                    // print("[-] Speed is setting to Zero");

                                    speed = 0;
                                }
                    }
                }
        }

        if (!moveToJumpPoint)
        {
            if (distanceFromPlayer > 4) // if not too close to the player shoot at them
            {
                CTO_Anim.SetBool("shootAtPlayer", true);

                if (canSeePlayer)
                {
                    Shoot();
                }
            } else if (distanceFromPlayer > 2.15f && distanceFromPlayer <= 4) // when in close range try to get within range for a melee
                {
                    CTO_Anim.SetBool("shootAtPlayer", false);
                } else if (distanceFromPlayer <= 2.15f) // within range to melee
                    {
                        Melee();
                    }
        }

        if (!moveToJumpPoint)
        {
            CTO_Anim.SetFloat("speed", speed);
        }

        agent.speed = speed;
    } // End Of Movement

    void SpeedAdjust(Vector3 Target)
    {
        float distFromTarget = Vector3.Distance(transform.position, Target);

        if (distFromTarget <= minDist) // when the agent is close to the player we switch the anim blend tree to play regular movements and not shooting anims
        {
            CTO_Anim.SetBool("canShoot", false);
            CTO_Anim.SetBool("shootAtPlayer", false);

            // walk towards destination
            if (speed > SpeedConst)
            {
                speed -= 0.05f;
            } else
                {
                    speed = SpeedConst;
                }
        } else if (distFromTarget > minDist) // agent plays shooting anims
            {
                CTO_Anim.SetBool("shootAtPlayer", true);

                // run towards destination
                if (speed < SpeedConst * 2.25f)
                {
                    speed += 0.05f;
                } else
                    {
                        speed = SpeedConst * 2.25f;
                    }

                if (canSeePlayer) // shoot the player when seen
                {
                    if (CTO_Anim.GetBool("canShoot"))
                    {
                        Shoot();
                    } else
                        {
                            Invoke("AllowShooting", 1f);
                        }
                }
            }

        // print("[+] Moving Towards Target : Dist " + distFromTarget + " [*] Speed => " + speed);

        CTO_Anim.SetFloat("speed", speed);
    }

    void AllowShooting()
    {
        CTO_Anim.SetBool("canShoot", true);
    }

//=======================================================================================
//===================================== JUMP ============================================
//=======================================================================================

    void FindClosestJumpUp_Point()
    {
        jumpScan = true;

        // print("[*] Looking Where To Jump Up To. . .");

        if (!jumpPointSet)
        {
            float[] distances = new float[jumpPaths.Length];

            for (int i = 0; i < jumpPaths.Length; i++)
            {
                if (Mathf.Abs(jumpPaths[i].GetStartOfJumpPath().y - transform.position.y) < 1)
                {
                    distances[i] = Vector3.Distance(jumpPaths[i].GetStartOfJumpPath(), Player.position);
                } else
                    {
                        distances[i] = 1000;
                    }
            }

            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] == Mathf.Min(distances))
                {
                    ind = i;
                }
            }

            // print("[+] Jump Up Position Found!");

            chosenIndex = ind;

            // print("[*] Path Set From " + jumpPaths[ind].transform.name );

            jumpPos = jumpPaths[ind].GetStartOfJumpPath();
            jumpPointSet = true;
        }
    }

    void FollowJumpUp_Path() // look in the direction of the jump and travel along the given Vector3 Path UP
    {
        Vector3[] positions = jumpPaths[ind].GetPathPoints();

        Vector3 LookPos = new Vector3(jumpPos.x, transform.position.y, jumpPos.z);
        transform.LookAt(LookPos);

        if (pathIndex < positions.Length)
        {
            if (transform.position == positions[pathIndex])
            {
                pathIndex++;
            } else
                {
                    transform.position = Vector3.MoveTowards(transform.position, positions[pathIndex], 20f * Time.deltaTime);
                }
        } else
            {
                LandedFromJump();
            }
    }

//========================================================================================

    void FindClosestJumpDown_Point()
    {
        jumpScan = true;

        // print("[*] Looking Where To Jump Down To. . .");

        if (!jumpPointSet)
        {
            float[] distances = new float[jumpPaths.Length];

            for (int i = 0; i < jumpPaths.Length; i++)
            {
                if (Mathf.Abs(jumpPaths[i].transform.position.y - transform.position.y) < 1)
                {
                    distances[i] = Vector3.Distance(jumpPaths[i].transform.position, Player.position);
                } else
                    {
                        distances[i] = 1000;
                    }
            }

            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] == Mathf.Min(distances))
                {
                    ind = i;
                }
            }

            // print("[+] Jump Down Position Found!");

            chosenIndex = ind;

            // print("[*] Path Set From " + jumpPaths[ind].transform.name );

            jumpPos = jumpPaths[ind].transform.position;
            jumpPointSet = true;
        }
    }

    void FollowJumpDown_Path() // look in the direction of the jump and travel along the given Vector3 Path DOWN
    {
        Vector3[] positions = jumpPaths[ind].GetPathPoints();

        Vector3 LookPos = new Vector3(jumpPos.x, transform.position.y, jumpPos.z);
        transform.LookAt(LookPos);

        if (pathIndex < positions.Length)
        {
            if (transform.position == positions[(positions.Length - 1) - pathIndex])
            {
                pathIndex++;
            } else
                {
                    transform.position = Vector3.MoveTowards(transform.position, positions[(positions.Length - 1) - pathIndex], 20f * Time.deltaTime);
                }
        } else
            {
                LandedFromJump();
            }
    }

//========================================================================================

    void JumpUpToDifferentGround() // play the jump anim when traveling the jump-path
    {
        activate = false;

        // print("[+] Jumping!");

        CTO_Anim.SetBool("landed", false);

        CTO_Anim.Play("jump");

        agent.enabled = false;
        jumpScan = false;

        pathIndex = 0;
    }

    void LandedFromJump() // when the agent lands we do a landing animation and transition back to attacking the player
    {
        CTO_Anim.SetBool("landed", true);

        agent.enabled = true;

        moveToJumpPoint = false;
        jumpPointSet = false;
        jumping = false;

        CTO_Activate();
    }

//=======================================================================================
//===================================== SHOOTING ========================================
//=======================================================================================

    void Shoot()
    {
        Vector3 Target = new Vector3(Player.position.x, Player.position.y + 0.55f, Player.position.z);
        
        NavMeshPath navMeshPath = new NavMeshPath();
        
        if (agent.CalculatePath(Target, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            if (shoot) // shoot at the player and continue moving to them
            {
                StartCoroutine(firingAtPlayer());
            }
        } else
            { 
                if (!moveToJumpPoint) // the player is in the air or at an unreachable destination that hasnt triggered the requirements to Jump
                {
                    speed = 0;
                }

                if (shoot) // shoot at the player even when we cant reach them
                {
                    StartCoroutine(firingAtPlayer());
                }
            }
    }

    IEnumerator firingAtPlayer() // rate of fire
    {
        shoot = false;
        muzzleFlash.Play();
        CTO_gunShot.Play();

        GameObject newBullet = Instantiate(bulletPref, bulletSpawn.position, bulletPref.transform.rotation);

        newBullet.transform.forward = (playerHead.position - newBullet.transform.position);

        newBullet = null;

        muzzleFlash.Stop();

        yield return new WaitForSeconds(0.1f);
        shoot = true;
    }

    void AimArmAtPlayer() // adjust the body to look at the player so the attacks look natural as can be
    {
        Vector3 LookPos = new Vector3(playerHead.position.x, transform.position.y, playerHead.position.z);

        middleSpine.LookAt(LookPos);
        upperSpine.LookAt(LookPos);

        leftUpperArm.up = (playerHead.position - leftUpperArm.position);
        leftLowerArm.up = (playerHead.position - leftLowerArm.position);

        CTO_Head.LookAt(playerHead.position);
    }
    
//=======================================================================================
//================================== EXTRA ==============================================
//=======================================================================================

    void Melee()
    {
        CTO_Anim.SetBool("shootAtPlayer", false);
        StopCoroutine(firingAtPlayer());
        CTO_Anim.Play("melee");

        // if he hits the player with a kick, disabled player movement and mouse scripts
        RaycastHit hit;
        Vector3 rayDir = (playerHead.position - CTO_Head.position).normalized;
        if (Physics.Raycast(CTO_Head.position, rayDir, out hit, playerLayerMask))
        {
            if (hit.transform.GetComponent<HealthScript>() != null)
            {
                hit.transform.GetComponent<HealthScript>().RagdollKnockDown();
            }
        }
    }

    void MissleLauncherAttack()
    {
        // fire a missle that follows the player

        if (firedMissile == null)
        {
            if (!shotObstructed)
            {
                missileLauncher_Sound.Play();
                firedMissile = Instantiate(MissilePref, missileSpawn.position, missileSpawn.rotation);
            }
        } else
            {
                firedMissile.GetComponent<Rigidbody>().velocity = firedMissile.transform.forward * missleGeneralSpeed;

                firedMissile.transform.LookAt(playerHead.position);
            }
    }

    void ReadyLauncher() // triggerd when at half health or less
    {
        missleLauncherReady = true;
    }

    void Death()
    {
        float currentHealth = (float) healthScript.GetHealth() / maxHealth; // ranges from 0 -> 1

        if (CTO_HealthBar != null) // HealthPivot
        {
            CTO_HealthBar.localScale = new Vector3(currentHealth, 1, 1);
        }

        // turns off the script on death
        if (healthScript.GetHealth() < 1)
        {
            activate = false;
            Destroy(CTO_HealthUI); // remove the UI component

            if (firedMissile != null)
            {
                Destroy(firedMissile);
            }

            CTO_Anim.enabled = false;
            LevelObjectiveScript.CTO_HasDied();
            this.enabled = false;
        } else
            {
                if (currentHealth <= 0.5f) // when half way dead we want to start using rockets and move faster
                {
                    MissleLauncherAnim.enabled = true;
                    if (!missleLauncherReady)
                    {
                        Invoke("ReadyLauncher", 0.65f);
                    }
                    SpeedConst = 6;
                }
            }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, minDist);
    }
}//EndScript