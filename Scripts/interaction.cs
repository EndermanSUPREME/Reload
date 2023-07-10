using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class interaction : MonoBehaviour
{
    [SerializeField] UnityEvent unityGameEvent = new UnityEvent();

    public void TriggerEvent()
    {
        unityGameEvent.Invoke();
    }

    public void DestroyInteractionComponent()
    {
        Destroy(this);
    }
}//EndScript