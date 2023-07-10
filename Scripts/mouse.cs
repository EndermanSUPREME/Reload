using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouse : MonoBehaviour
{
    public float sensitivity = 200f;
    float xRotation = 0f, mouseX, mouseY;
    public Transform Player;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        Invoke("FixSensitivity", 0.25f);
    }

    void FixSensitivity()
    {
        sensitivity = ((float) PlayerPrefs.GetInt("Sensitivity"));
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = 65;

        mouseX = (Input.GetAxis("Remote X") + Input.GetAxis("Mouse X")) * sensitivity * Time.deltaTime;
        mouseY = (Input.GetAxis("Remote Y") + Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        Player.Rotate(Vector3.up * mouseX);
    }

    public void SetSensitivity(float s)
    {
        sensitivity = s;
    }
    
}//EndScript