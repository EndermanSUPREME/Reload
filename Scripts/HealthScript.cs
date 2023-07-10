using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class HealthScript : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] bool alerted = false, Hostile, isPod_Gaurd = false;
    [SerializeField] Text HealthDisplay;
    bool knockedBack = false, engageControllerPhysics = false, droppedABox = false, isDead = false, regeneratingHealth = false;
    float verticleMotion = 10;
    
    [SerializeField] Rigidbody[] rbs;
    [SerializeField] LimbInfo[] LimbScripts;
    public GameObject ammoBox;
    
    robotAIScript botScript;
    [SerializeField] Animator roboAnim;
    [SerializeField] Animator playerArms;

    [SerializeField] NavMeshAgent roboAgent;

    [SerializeField] Transform robotTrigger, playerGroundCheck;
    [SerializeField] LayerMask groundMask;

    [SerializeField] movement keyboardScript;
    [SerializeField] mouse mouseScript;
    [SerializeField] Weapons attackScript;
    
    [SerializeField] CharacterController playerController;

    [SerializeField] AudioSource Injured_I, Injured_II;

    void Awake()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        LimbScripts = GetComponentsInChildren<LimbInfo>();

        if (transform.name == "robot" || transform.name == "CTO_BotObj")
        {
            Hostile = true;
        } else
            {
                Hostile = false;
            }

        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = true;
        }
    }

    void Start()
    {
        if (transform.GetComponent<robotAIScript>() != null)
        {
            botScript = transform.GetComponent<robotAIScript>();
            
            roboAnim = transform.GetComponent<Animator>();
            roboAgent = transform.GetComponent<NavMeshAgent>();
            robotTrigger.GetComponent<Collider>().enabled = false;
        }

        if (transform.GetComponent<CTO_BossScript>() != null)
        {
            roboAnim = transform.GetComponent<Animator>();
            roboAgent = transform.GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        if (engageControllerPhysics)
        {
            CharacterController_KnockBack_Physics();
        }

        if (HealthDisplay != null)
        {
            HealthDisplay.text = health + "%";
        }

        if (isDead)
        {
            health = 0;
            StopCoroutine(Regen());
        }
    }

    void CharacterController_KnockBack_Physics()
    {
        if (playerController != null)
        {
            if (knockedBack)
            {
                playerArms.enabled = false;
    
                keyboardScript.enabled = false;
                mouseScript.enabled = false;
                attackScript.enabled = false;
    
                playerController.Move((-playerController.transform.forward * 10 + playerController.transform.up * verticleMotion) * Time.deltaTime);
                verticleMotion -= 0.5f;

                if (Physics.CheckSphere(playerGroundCheck.position, 0.4f, groundMask))
                {
                    knockedBack = false;
                }
            } else
                {
                    playerArms.enabled = true;
    
                    keyboardScript.enabled = true;
                    mouseScript.enabled = true;
                    attackScript.enabled = true;

                    engageControllerPhysics = false;
                }
        }
    }

    IEnumerator Regen()
    {
        health += 5;

        yield return new WaitForSeconds(2.5f);

        if (health < 80)
        {
            StartCoroutine(Regen());
        } else
            {
                regeneratingHealth = false;
                StopCoroutine(Regen());
            }
    }

    public void TakeDamage(int amount)
    {
        if (!isDead)
        {
            health -= amount;

            if (health <= 50 && playerController != null && !regeneratingHealth) // when players half way dead
            {
                regeneratingHealth = true;
                StartCoroutine(Regen());
            } else if (playerController != null && regeneratingHealth)
                {
                    regeneratingHealth = false;
                    StopCoroutine(Regen());
                }

            if (health <= 0)
            {
                StopCoroutine(Regen());
                
                Death();
            }
        }

        if (!isDead)
        {
            int choice = Random.Range(0, 50);

            if (choice % 2 == 0)
            {
                Injured_I.Play();
            } else
                {
                    Injured_II.Play();
                }

            if (!alerted)
            {
                Death();
            } else
                {
                    if (botScript != null)
                    {
                        botScript.AttackedByPlayer();
                    }
                }
        }
    }

    public void CEO_DamageToggle(bool value)
    {
        if (value)
        {
            for (int i = 0; i < LimbScripts.Length; i++)
            {
                LimbScripts[i].SetDamage(1000);
            }
        } else
            {
                for (int i = 0; i < LimbScripts.Length; i++)
                {
                    LimbScripts[i].SetDamage(0);
                }
            }
    }

    public void Alerted()
    {
        alerted = true;
    }

    public void ExplosiveDeath()
    {
        Death();
    }

    public void RagdollKnockDown()
    {
        engageControllerPhysics = true;
        knockedBack = true;
        verticleMotion = 10;
    }

    void Death()
    {
        isDead = true;

        health = 0;

        if (roboAnim != null && roboAgent != null)
        {
            roboAnim.enabled = false;
            roboAgent.enabled = false;
        }

        if (botScript != null)
        {
            botScript.enabled = false;

            for (int i = 0; i < LimbScripts.Length; i++)
            {
                LimbScripts[i].enabled = false;
            }

            robotTrigger.gameObject.layer = 8;
            robotTrigger.GetComponent<Collider>().enabled = true;

            if (Hostile && !droppedABox)
            {
                GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>().EnemyKilled();

                if (!isPod_Gaurd && (SceneManager.GetActiveScene().buildIndex == 8 || SceneManager.GetActiveScene().buildIndex == 9))
                {
                    GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>().AlertAllBots();
                }

                if (ammoBox != null)
                {
                    Instantiate(ammoBox, transform.position, ammoBox.transform.rotation);
                    droppedABox = true;
                }
            }
        }

        if (keyboardScript != null)
        {
            GameObject.Find("PlayerCam").GetComponent<SaveGameData>().BuildSaveFile();

            playerArms.enabled = false;
            playerController.enabled = false;

            keyboardScript.enabled = false;
            mouseScript.enabled = false;
            attackScript.enabled = false;

            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex);
            
            // if (SceneManager.GetActiveScene().buildIndex > 8) // any scene from outside prison and after
            // {
            //     Destroy(GameObject.Find("playerBody"), 1.9f);
            // }
        }

        // player or enemy dies (ragdoll)
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = false;

            rbs[i].AddForce(-rbs[i].transform.forward * 300);
            rbs[i].AddForce(rbs[i].transform.up * 300);
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public bool isThisPlayerDead()
    {
        return isDead;
    }
}//EndScript