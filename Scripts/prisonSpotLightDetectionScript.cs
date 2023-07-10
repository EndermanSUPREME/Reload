using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prisonSpotLightDetectionScript : MonoBehaviour
{   
    [SerializeField] LayerMask playerMask;
    [Range(1,4)] public float animChoice;
    public Animator anim;

    void Start()
    {
        anim.SetFloat("AnimChoice", animChoice);
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (Physics.CheckSphere(hit.point, 10, playerMask))
            {
                GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>().AlertAllBots();
            }
        }
    }

    void OnDrawGizmos()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hit.point, 10);
        }
    }
}//EndScript