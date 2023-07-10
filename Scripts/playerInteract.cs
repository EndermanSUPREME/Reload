using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerInteract : MonoBehaviour
{
    [SerializeField] GameObject DisplayInteraction;
    public Text UI_InteractionText;

    void Update()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            UI_InteractionText.text = "X";
        } else
            {
                UI_InteractionText.text = "E";
            }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2))
        {
            if (hit.transform.GetComponent<interaction>() != null)
            {
                DisplayInteraction.SetActive(true);

                if (Input.GetKeyDown(KeyCode. E) || Input.GetKeyDown("joystick button 2")) // E key or X button
                {
                    if (hit.transform.GetComponent<interaction>() != null)
                    {
                        hit.transform.GetComponent<interaction>().TriggerEvent();
                    }
                }
            } else
                {
                    DisplayInteraction.SetActive(false);
                }
        } else
            {
                DisplayInteraction.SetActive(false);
            }
    }
    
}//EndScript