using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flowerBedFiller : MonoBehaviour
{
    public float boundX, boundY;
    public int numberOfObjectsToSpawn;
    public GameObject[] flowerBedObjects;

    void Start()
    {
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            GameObject spawnedObject = flowerBedObjects[Random.Range(0, flowerBedObjects.Length)];

            float x = transform.position.x + Random.Range(-boundX / 2, boundX / 2);
            float z = transform.position.z + Random.Range(-boundY / 2, boundY / 2);

            GameObject obj = Instantiate(spawnedObject, new Vector3(x, 0, z), transform.rotation);
            obj.transform.parent = transform;
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0.05f);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(boundX, 0.5f, boundY));
    }
}//EndScript