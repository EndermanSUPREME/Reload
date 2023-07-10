using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBuilder : MonoBehaviour
{
    [SerializeField] GameObject Object;
    public GameObject[] ObjectArray;
    public int numberOfObjectsToPlace;
    public bool build = false, X, Y, Z;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Object.transform.position, 0.5f);

        if (build)
        {
            if (ObjectArray.Length < 1)
            {
                ObjectArray = new GameObject[numberOfObjectsToPlace];
            } else
                {
                    for (int i = 0; i < ObjectArray.Length; i++)
                    {
                        DestroyImmediate(ObjectArray[i]);
                    }

                    ObjectArray = new GameObject[numberOfObjectsToPlace];
                }

            if (X)
            {
                for (int i = 0; i < ObjectArray.Length; i++)
                {
                    ObjectArray[i] = Instantiate(Object, Object.transform.position, Object.transform.rotation);

                    ObjectArray[i].transform.parent = transform;
                    ObjectArray[i].transform.localScale = new Vector3(1, 1, 1);
                    ObjectArray[i].transform.localPosition = Object.transform.localPosition + new Vector3(Object.transform.localPosition.x * (i + 1), 0, 0);
                }
            }

            if (Y)
            {
                for (int i = 0; i < ObjectArray.Length; i++)
                {
                    ObjectArray[i] = Instantiate(Object, Object.transform.position, Object.transform.rotation);

                    ObjectArray[i].transform.parent = transform;
                    ObjectArray[i].transform.localScale = new Vector3(1, 1, 1);
                    ObjectArray[i].transform.localPosition = Object.transform.localPosition + new Vector3(0, Object.transform.position.y * (i + 1), 0);
                }
            }

            if (Z)
            {
                for (int i = 0; i < ObjectArray.Length; i++)
                {
                    ObjectArray[i] = Instantiate(Object, Object.transform.position, Object.transform.rotation);

                    ObjectArray[i].transform.parent = transform;
                    ObjectArray[i].transform.localScale = new Vector3(1, 1, 1);
                    ObjectArray[i].transform.localPosition = Object.transform.localPosition + new Vector3(0, 0, Object.transform.position.z * (i + 1));
                }
            }

            build = false;
        }
    }
}//EndScript