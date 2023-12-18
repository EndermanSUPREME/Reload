using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CEO_Script : MonoBehaviour
{
    bool pacingHelipad = true, shoot = true, CEO_Alive = true, audioFadeTriggered = false;
    Transform playerHead, playerBody;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform[] pacePositions;
    [SerializeField] Transform middleSpine, bulletSpawn, playerPosAfterEvent;
    [SerializeField] GameObject bulletPref, CEO_Cutscene;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Animator CEO_Anim;
    [SerializeField] HealthScript CEO_HealthScript;
    [SerializeField] movement playerMovementScript;
    [SerializeField] float distToEnabledDamage;

    [SerializeField] AudioSource CEO_gunShotSound; 

    void Start()
    {
        Invoke("Config_CEO", 0.75f);
    }

    void Config_CEO()
    {
        CEO_HealthScript.CEO_DamageToggle(false); // limbs damage int set to 0

        playerHead = GameObject.Find("PlayerCam").transform;
        playerBody = GameObject.Find("playerBody").transform;

        playerMovementScript = playerBody.GetComponent<movement>();

        agent.SetDestination(pacePositions[Random.Range(0, pacePositions.Length)].position);
    }

    // Update is called once per frame
    void Update()
    {
        if (CEO_Alive)
        {
            if (pacingHelipad)
            {
                Pacing();
            } else
                {
                    AttackAtPlayer();
    
                    CEO_Anim.Play("shooting");
    
                    if (CEO_Alive && CEO_HealthScript.GetHealth() < 1)
                    {
                        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();

                        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn();
                        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().SetNextInt(SceneManager.GetActiveScene().buildIndex + 1);

                        CEO_Alive = false;
                    }
                }
        } else
            {
                CEO_gunShotSound.Stop();

                if (!audioFadeTriggered)
                {
                    AudioFades AudioShifter = (AudioFades)FindObjectOfType(typeof(AudioFades));
                    
                    if (AudioShifter != null)
                    {
                        AudioShifter.FadeAudioOut();
                    }

                    audioFadeTriggered = true;
                }
            }
    }

    // Runs when the Object is set to active after being disabled
    void OnEnable()
    {
        if (!pacingHelipad)
        {
            StartCoroutine(firingAtPlayer());
        }
    }

    void LateUpdate()
    {
        if (!pacingHelipad)
        {
            middleSpine.LookAt(playerHead.position);
        }
    }

    void Pacing()
    {
        Vector3 EndPt = new Vector3(agent.destination.x, 0, agent.destination.z);
        Vector3 AIPos = new Vector3(agent.transform.position.x, 0, agent.transform.position.z);

        if (Vector3.Distance(AIPos, EndPt) < 0.05f)
        {
            agent.SetDestination(pacePositions[Random.Range(0, pacePositions.Length)].position);
        }
    }

    void AttackAtPlayer()
    {
        if (shoot)
        {
            StartCoroutine(firingAtPlayer());
        }

        Vector3 CEO_Pos = new Vector3(agent.transform.position.x, 0, agent.transform.position.z);
        Vector3 Player_Pos = new Vector3(playerBody.position.x, 0, playerBody.position.z);

        if (Vector3.Distance(CEO_Pos, Player_Pos) <= distToEnabledDamage)
        {
            CEO_HealthScript.CEO_DamageToggle(true); // limbs damage int set to 200
        }
    }
    
    IEnumerator firingAtPlayer()
    {
        shoot = false;
        muzzleFlash.Play();
        CEO_gunShotSound.Play();

        GameObject newBullet = Instantiate(bulletPref, bulletSpawn.position, bulletPref.transform.rotation);

        newBullet.transform.forward = (playerHead.position - newBullet.transform.position);

        newBullet = null;

        muzzleFlash.Stop();

        yield return new WaitForSeconds(0.1f);
        shoot = true;
    }

    void LoadNextSection() // maybe run cutscene at this method, reposition player for reload
    {
        Invoke("PlayEventCutscene", 2); // length of fade

        if (playerPosAfterEvent != null)
        {
            print("[+] Position Changed");
            playerBody.transform.position = playerPosAfterEvent.position;
        }
    }

    void PlayEventCutscene()
    {
        if (playerHead != null)
        {
            playerHead.gameObject.SetActive(false);
        }

        if (CEO_Cutscene != null)
        {
            CEO_Cutscene.SetActive(true);
        }

        Invoke("CEO_Event", 7.1f); // length of animation
    }

    void CEO_Event() // player can engage in the final section of the level
    {
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();

        if (CEO_Cutscene != null)
        {
            CEO_Cutscene.SetActive(false);
        }

        if (playerHead != null)
        {
            playerHead.gameObject.SetActive(true);
        }

        GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeOut();

        playerHead.GetComponent<AudioFades>().ChangeMusicTrack(1);

        playerMovementScript.enabled = true;
        // GameObject.FindObjectOfType<UI_Interface>().enabled = true;

        pacingHelipad = false;

        agent.SetDestination(pacePositions[8].position);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform == GameObject.Find("PlayerCam").transform)
        {
            if (playerMovementScript != null)
            {
                print("[-] Cant Move");
                // GameObject.FindObjectOfType<UI_Interface>().enabled = false;
                playerMovementScript.enabled = false;
            }

            Destroy(GameObject.Find("Level_AI"));

            transform.GetComponent<Collider>().isTrigger = false;

            GameObject.Find("ScreenFade").GetComponent<LevelTransitioning>().FadeIn_Event(); // maybe run a cutscene

            Invoke("LoadNextSection", 2);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (agent != null)
        {
            Gizmos.DrawWireSphere(agent.transform.position, distToEnabledDamage);
        }
    }
}//EndScript