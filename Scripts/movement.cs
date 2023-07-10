using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class movement : MonoBehaviour
{
    [SerializeField] float speedConst, jumpHeight, currentSpeed;
    Vector3 velocity, headNormalPos;
    [SerializeField] bool grounded, headObstruction = false;
    float gravity = -9.81f, groundDistance = 0.4f, speed, x, z;
    [SerializeField] CharacterController controller;
    [SerializeField] Transform groundCheck, playerCam, crouchTar, headObstruct, playerArms;
    [SerializeField] LayerMask groundMask, headMask;

    [SerializeField] Animator anim;

    [SerializeField] AudioSource footStepOne, footStepTwo, jumpSound, landingSound;
    bool landedJump = true, replayWalkSound = true, controllerSprint = false;

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = 65;

        basicMovement();
        jumpMechanic();
        CrouchMechanic();

    } //End of Update

    void basicMovement()
    {
        bool space = Input.GetKeyDown(KeyCode. Space);
        
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");

            if (Input.GetKeyDown("joystick button 8"))
            {
                if (!controllerSprint)
                {
                    controllerSprint = true;
                } else
                    {
                        controllerSprint = false;
                    }
            }

            if ((Input.GetKey(KeyCode. LeftShift) || controllerSprint) && (!Input.GetKey(KeyCode. LeftControl) || !Input.GetKey("joystick button 1")) && z > 0) // B button on controller
            {
                if (speed < speedConst * 2.05f)
                {
                    speed += 0.035f;
                } else
                    {
                        speed = speedConst * 2.05f;
                    }

                ArmDisplacement(x * 0.07f, -z * 0.07f); // 0.05 -> 0.07 ? x & z
            } else
                {
                    controllerSprint = false;

                    if (speed > speedConst)
                    {
                        speed -= 0.035f;
                    } else
                        {
                            speed = speedConst;
                        }

                    playerArms.localPosition = Vector3.MoveTowards(playerArms.localPosition, new Vector3(0, -0.17f, 0), 0.25f * Time.deltaTime);
                };

            currentSpeed = z * speed;

            Vector3 move = transform.right * x + transform.forward * z;

            if ((Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0) && grounded)
            {
                if (!Input.GetKey(KeyCode. LeftShift) && !controllerSprint)
                {
                    if (replayWalkSound)
                    {
                        StartCoroutine(footSteps(0.5f));
                    }
                } else
                    {
                        if (replayWalkSound)
                        {
                            StartCoroutine(footSteps(0.25f));
                        }
                    }
            } else
                {
                    StopCoroutine(footSteps(0));
                }

            if (controller.enabled)
            {
                controller.Move(move * speed * Time.deltaTime);

                velocity.y += gravity * Time.deltaTime;

                controller.Move(velocity * Time.deltaTime);
            }
    }

    IEnumerator footSteps(float t)
    {
        replayWalkSound = false;

        footStepOne.Play();
        yield return new WaitForSeconds(t);
        footStepTwo.Play();
        yield return new WaitForSeconds(t);
        replayWalkSound = true;
    }

    void ArmDisplacement(float xShift, float zShift)
    {
        Vector3 shiftPos = new Vector3(xShift, -0.17f, zShift);

        // Vector3 normalPos = new Vector3(0, -0.17f, 0);

        playerArms.localPosition = Vector3.MoveTowards(playerArms.localPosition, shiftPos, 0.25f * Time.deltaTime);
    }

    void jumpMechanic()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (grounded && velocity.y < 0)
        {
            velocity.y = -4f;
        };
       
        if (PlayerPrefs.GetInt("isGamePaused") == 0)
        {
            if (Input.GetButtonDown("Jump") && grounded)
            {
                jumpSound.Play();
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            };
        }

        if (!grounded)
        {
            Invoke("inTheAir", 0.25f);
        }

        if (grounded)
        {
            if (!landedJump)
            {
                if (Mathf.Abs(z) < 0.75f)
                {
                    landedJump = true;
                    // landingSound.Play();
                }
            }
        }
    }

    void inTheAir()
    {
        landedJump = false;
    }

    void CrouchMechanic()
    {
        if (!headObstruction)
        {
            if (Input.GetKey(KeyCode. LeftControl) || Input.GetKey("joystick button 1")) // crouch
            {
                headObstruction = Physics.CheckSphere(headObstruct.position, 0.45f, headMask);
                playerCam.position = Vector3.MoveTowards(playerCam.position, crouchTar.position, 5 * Time.deltaTime);

                controller.height = 1;
                controller.center = new Vector3(0, -0.5f, 0);
            } else // stand up
                {
                    playerCam.localPosition = Vector3.MoveTowards(playerCam.localPosition, new Vector3(0, 0.5f, 0), 5 * Time.deltaTime);

                    controller.height = 2;
                    controller.center = new Vector3(0, 0, 0);
                }
        } else if (headObstruction && !Input.GetKey(KeyCode. LeftControl))
            {
                if (!Physics.CheckSphere(headObstruct.position, 0.45f, headMask))
                {
                    headObstruction = false;
                }

                playerCam.position = Vector3.MoveTowards(playerCam.position, crouchTar.position, 5 * Time.deltaTime);

                controller.height = 1;
                controller.center = new Vector3(0, -0.5f, 0);
            }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(headObstruct.position, 0.45f);
    }

}//EndScript