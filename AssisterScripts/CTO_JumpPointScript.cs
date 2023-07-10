using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CTO_JumpPointScript : MonoBehaviour
{
    public bool showJumpPath = false, testingVelocityMath = false, generateVectors = false;
    [SerializeField] GameObject ball, emptyPoint;
    GameObject spawnedBall;
    [SerializeField] Vector3[] pointsOnPath;
    [SerializeField] Transform[] spawnedTransforms;
    public float ballLifeTime, initial_X, initial_Y;

    IEnumerator SpawnPathPoint()
    {
        GameObject spawnedPoint = Instantiate(emptyPoint, spawnedBall.transform.position, transform.rotation);
        spawnedPoint.transform.parent = transform;

        yield return new WaitForSeconds(0.05f);

        if (spawnedBall != null)
        {
            StartCoroutine(SpawnPathPoint());
        } else
            {
                StopCoroutine(SpawnPathPoint());
            }
    }

    void Start()
    {
        for (int i = 0; i < pointsOnPath.Length; i++)
        {
            pointsOnPath[i].z = transform.position.z;
        }
    }

    void Update()
    {
        if (showJumpPath)
        {
            spawnedBall = Instantiate(ball, transform.position, transform.rotation);
            spawnedBall.transform.parent = transform;
            spawnedBall.GetComponent<Rigidbody>().velocity = new Vector3(initial_X, initial_Y, 0);

            if (!testingVelocityMath)
            {
                StartCoroutine(SpawnPathPoint());
            }

            Invoke("deleteBall", ballLifeTime);

            showJumpPath = false;
        }

        if (generateVectors)
        {
            PlotPathPoints();
            generateVectors = false;
        }
    }

    public Vector3[] GetPathPoints()
    {
        return pointsOnPath;
    }

    public Vector3 GetStartOfJumpPath()
    {
        return pointsOnPath[0];
    }

    void PlotPathPoints()
    {
        pointsOnPath = new Vector3[spawnedTransforms.Length];

        for (int i = 0; i < spawnedTransforms.Length; i++)
        {
            pointsOnPath[(spawnedTransforms.Length - 1) - i] = spawnedTransforms[i].position;
        }
    }

    void deleteBall()
    {
        // print("[+] Ball Deleted");
        Destroy(spawnedBall);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.1f);

        Debug.DrawRay(transform.position, transform.forward, Color.red);

        if (pointsOnPath != null)
        {
            Gizmos.color = Color.green;
    
            for (int i = 0; i < pointsOnPath.Length; i++)
            {
                Gizmos.DrawSphere(pointsOnPath[i], 0.1f);
            }

            Start();
        }
    }
}//EndScript