using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbInfo : MonoBehaviour
{
    [SerializeField] HealthScript healthScript;
    [SerializeField] int damagePoint;
    int LimbMaskInt = 10;

    void Start()
    {
        if (transform.GetComponent<Camera>() == null)
        {
            gameObject.layer = LimbMaskInt;
        }
    }

    public void LimbInjured()
    {
        healthScript.TakeDamage(damagePoint);
    }

    public void SetDamage(int amount)
    {
        damagePoint = amount;
    }
}//EndScript