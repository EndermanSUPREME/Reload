using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pod_BotScript : MonoBehaviour
{
    [SerializeField] Transform[] DestinationPoints;
    NavMeshAgent agent;
    public Transform showersDestinationPoint;
    bool searchingForPlayer = false;

    public float speedConst;
    float speed;
    public Animator robotAnim;
    public HealthScript healthScript;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        searchingForPlayer = false;

        if (agent != null)
        {
            agent.SetDestination(DestinationPoints[Random.Range(0, DestinationPoints.Length)].position);
        }
    }

    void Update()
    {
        UpdateAnimator();
        AdjustSpeed();

        if (!agent.enabled)
        {
            GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
            this.enabled = false;
        }

        if (!searchingForPlayer)
        {
            float dist = Vector3.Distance(transform.position, agent.destination);
    
            if (dist < 0.1f)
            {
                StartCoroutine(IdleDelay());
            }
        } else
            {
                agent.SetDestination(showersDestinationPoint.position);
            }
    }

    public void ResetBot()
    {
        searchingForPlayer = false;
    }

    IEnumerator IdleDelay()
    {
        agent.speed = 0;

        yield return new WaitForSeconds(1);

        if (!searchingForPlayer)
        {
            agent.SetDestination(DestinationPoints[Random.Range(0, DestinationPoints.Length)].position);
        } else
            {
                agent.SetDestination(showersDestinationPoint.position);
            }
    }

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

    public void GaurdSearchForPlayer() //  called via prison script
    {
        GameObject.Find("BotNetAlert").GetComponent<LevelObjectiveList>().ObjectiveCompleted();
        searchingForPlayer = true;
    }
}//EndScript