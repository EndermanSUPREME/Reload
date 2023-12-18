using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Weapons : MonoBehaviour
{
    [Range(-1, 4)] public int weaponEquipped, scroll_Int;
    [SerializeField] GameObject Arms, knife, pistol, rifle, bulletHolePref, throwingKnifePref, grenadePref;
    [SerializeField] GameObject PistolOnPlayer, RifleOnPlayer, KnifeOnPlayer;
    [SerializeField] ParticleSystem pistolFlash;
    [SerializeField] Animator armsAnim, hitMarker, critMarker;
    bool rifleDelay = false, quietThrowable = true, throwing = false, globalAlert = false, produceShotSound = true, reloading = false, dpadPress = false, scrollUp = false, scrollDown = false;
    bool rightTriggerDown = false, leftTriggerDoWeapons = false, lt_use = false, rt_use = false, attackOnce = false, throwOnce = false, dpadPress2 = false, dpad_swap = false;

    [SerializeField] Text AmmoCountDisplay, throwableDisplay;
    [SerializeField] int totalPistolAmmo, pistolMagCount, totalRifleAmmo, rifleMagCount;

    [SerializeField] Transform throwPoint;
    [SerializeField] [Range(0, 8)] int throwingKnifeTotal;
    [SerializeField] [Range(0, 5)] int grenadeTotal;
    [SerializeField] float throwingPower;

    LevelWideAlertness BotNetAlert;
    [SerializeField] ReloadAnimationAssistance reloadAssist;

    [SerializeField] AudioSource knifeEquip, pistolEquip, rifleEquip, knifeSwoop, pistolShot, rifleShot;

    // Start is called before the first frame update
    void Start()
    {
        globalAlert = false;

        GameObject botnetScripObj = GameObject.Find("BotNetAlert");
        BotNetAlert = botnetScripObj.GetComponent<LevelWideAlertness>();

        reloadAssist = GameObject.FindObjectOfType<ReloadAnimationAssistance>();

        weaponEquipped = 0;

        if (SceneManager.GetActiveScene().buildIndex > 8) // outside prison and after
        {
            Invoke("GiveGearFromSave", 0.5f);
        }
    }

    public void SetLevelAmmoDefault(int pMag, int pO, int rMag, int rO)
    {
        pistolMagCount = pMag;
        totalPistolAmmo = pO;
        
        rifleMagCount = rMag;
        totalRifleAmmo = rO;
    }

    public void TryGiftGearFromTransfer() // player transfers thru scenes
    {
        AquireKnife();

        if (SceneManager.GetActiveScene().buildIndex == 11 && PlayerPrefs.GetInt("HasPistol") != 1 && PlayerPrefs.GetInt("HasRifle") != 1)
        {
            GiftPlayerWeaponForLazyPerformance();
        }
    }

    public void GiveGearFromSave() // player loaded save file
    {
        AquireKnife();
        
        if (PlayerPrefs.GetInt("HasPistol") == 1)
        {
            AquirePistol();
        }

        if (PlayerPrefs.GetInt("HasRifle") == 1)
        {
            AquireRifle();
        }

        if (SceneManager.GetActiveScene().buildIndex == 11)
        {
            if (PlayerPrefs.GetInt("HasPistol")  == 0 && PlayerPrefs.GetInt("HasRifle") == 0)
            {
                GiftPlayerWeaponForLazyPerformance();
            }
        }
    }

    void GiftPlayerWeaponForLazyPerformance()
    {
        AquireRifle();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<UI_Interface>().NotCutsceneArea())
        {
            if (SceneManager.GetActiveScene().buildIndex == 10)
            {
                scroll_Int = 0;
                weaponEquipped = scroll_Int;
                Sheath();
            }
    
            if (!throwing)
            {
                EquippedWeapon();
            } else
                {
                    if (pistol != null)
                    {
                        pistol.SetActive(false);
                    }
    
                    if (knife != null)
                    {
                        knife.SetActive(false);
                    }
    
                    if (rifle != null)
                    {
                        rifle.SetActive(false);
                    }
                }
    
            ThrowableWeapon();
        }
    }

    void ThrowableWeapon()
    {
        if (quietThrowable)
        {
            throwableDisplay.text = "Quiet : " + throwingKnifeTotal;
        } else
            {
                throwableDisplay.text = "Loud : " + grenadeTotal;
            }

        if (weaponEquipped > 0)
        {
            if (!dpadPress2 && Input.GetAxis("DPad X") != 0)
            {
                dpadPress2 = true;
                dpad_swap = true;
            }

            if (Input.GetAxis("DPad X") == 0)
            {
                dpadPress2 = false;
            }

            if (Input.GetKeyDown(KeyCode. Tab) || dpad_swap)
            {
                dpad_swap = false;

                if (quietThrowable)
                {
                    quietThrowable = false;
                } else
                    {
                        quietThrowable = true;
                    }
            }

            if (!lt_use && Input.GetAxis("leftTrigger") > 0.75f)
            {
                lt_use = true;
                throwOnce = true;
            }

            if (Input.GetAxis("leftTrigger") < 0.5f)
            {
                lt_use = false;
            }

            if ((Input.GetButtonDown("Fire2") || throwOnce) && !reloading)
            {
                throwOnce = false;
                throwing = true;

                armsAnim.Play("throw");

                if (quietThrowable)
                {
                    // throw knife
                    if (throwingKnifeTotal > 0)
                    {
                        GameObject thrownKnife = Instantiate(throwingKnifePref, throwPoint.position, Quaternion.identity);
                        thrownKnife.GetComponent<Rigidbody>().velocity = throwPoint.forward * throwingPower;
                        throwingKnifeTotal--;
                    }
                } else
                    {
                        // throw grenade
                        if (grenadeTotal > 0)
                        {
                            GameObject thrownGrenade = Instantiate(grenadePref, throwPoint.position, Quaternion.identity);
                            thrownGrenade.GetComponent<Rigidbody>().velocity = throwPoint.forward * throwingPower;
                            grenadeTotal--;
                        }
                    }
            }
        }

        if (armsAnim.GetCurrentAnimatorStateInfo(0).IsName("throw") && armsAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f)
        {
            throwing = false;
        }
    }

    public void PickUpKnife()
    {
        if (throwingKnifeTotal < 8)
        {
            throwingKnifeTotal++;
        }
    }

    public void PickUpGrenade()
    {
        if (grenadeTotal < 5)
        {
            grenadeTotal++;
        }
    }

//====================================================================================

    void EquippedWeapon()
    {
        bool Alpha1 = Input.GetKeyDown(KeyCode. Alpha1);
        bool Alpha2 = Input.GetKeyDown(KeyCode. Alpha2);
        bool Alpha3 = Input.GetKeyDown(KeyCode. Alpha3);
        bool Alpha4 = Input.GetKeyDown(KeyCode. Alpha4);

        // if (!dpadPress && Input.GetAxis("DPad Y") > 0)
        // {
        //     dpadPress = true;
        //     scrollUp = true;
        // }

        // if (!dpadPress && Input.GetAxis("DPad Y") < 0)
        // {
        //     dpadPress = true;
        //     scrollDown = true;
        // }

        // if (Input.GetAxis("DPad Y") == 0)
        // {
        //     dpadPress = false;
        // }

        scrollDown = Input.GetKeyDown("joystick button 4");
        scrollUp = Input.GetKeyDown("joystick button 5");

        if (Input.GetAxis("Mouse ScrollWheel") > 0f || scrollUp) // forward
        {
            scrollUp = false;
            scroll_Int++;

                switch (scroll_Int)
                {
                    case 0: // unarmed
                        weaponEquipped = scroll_Int;
                        Sheath();
                    break;
                    case 1: // knife
                        if (knife != null)
                        {
                            Ready_Knife();
                        }

                        if (knife == null && pistol == null && rifle == null) // completely unarmed
                        {
                            Sheath();
                            scroll_Int = 0;
                        }

                        weaponEquipped = scroll_Int;
                    break;
                    case 2: // pistol
                        if (pistol != null)
                        {
                            Ready_Pistol();
                        } else
                            {
                                if (rifle != null)
                                {
                                    Ready_Rifle();
                                    scroll_Int = 3;
                                }
                            }

                        if (knife == null && pistol == null && rifle == null) // completely unarmed
                        {
                            Sheath();
                            scroll_Int = 0;
                        }

                        weaponEquipped = scroll_Int;
                    break;
                    case 3: // rifle
                        if (rifle != null)
                        {
                            Ready_Rifle();
                        } else
                            {
                                Sheath();
                                scroll_Int = 0;
                            }

                        if (knife == null && pistol == null && rifle == null) // completely unarmed
                        {
                            Sheath();
                            scroll_Int = 0;
                        }

                        weaponEquipped = scroll_Int;
                    break;
                    case 4: // back to unarmed
                        scroll_Int = 0;
                        weaponEquipped = scroll_Int;
                        Sheath();
                    break;

                    default:
                    break;
                }
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f || scrollDown) // backward
            {
                scrollDown = false;
                scroll_Int--;

                switch (scroll_Int)
                {
                    case -1: // back to rifle
                        scroll_Int = 3;

                        if (rifle != null)
                        {
                            Ready_Rifle();
                        }

                        if (knife == null && pistol == null && rifle == null) // completely unarmed
                        {
                            Sheath();
                            scroll_Int = 0;
                        }

                        weaponEquipped = scroll_Int;
                    break;
                    case 0: // back to unarmed
                        scroll_Int = 0;
                        weaponEquipped = scroll_Int;
                        Sheath();
                    break;
                    case 1: // knife
                        if (knife != null)
                        {
                            Ready_Knife();
                        }

                        if (knife == null && pistol == null && rifle == null) // completely unarmed
                        {
                            Sheath();
                            scroll_Int = 0;
                        }

                        weaponEquipped = scroll_Int;
                    break;
                    case 2: // pistol
                        if (pistol != null)
                        {
                            Ready_Pistol();
                        } else
                            {
                                if (knife != null)
                                {
                                    Ready_Knife();
                                    scroll_Int = 1;
                                }
                            }

                        if (knife == null && pistol == null && rifle == null) // completely unarmed
                        {
                            Sheath();
                            scroll_Int = 0;
                        }

                        weaponEquipped = scroll_Int;
                    break;
                    case 3: // rifle
                        if (rifle != null)
                        {
                            Ready_Rifle();
                        } else
                            {
                                if (pistol != null)
                                {
                                    Ready_Pistol();
                                    scroll_Int = 2;
                                }
                            }

                        if (knife == null && pistol == null && rifle == null) // completely unarmed
                        {
                            Sheath();
                            scroll_Int = 0;
                        }

                        weaponEquipped = scroll_Int;
                    break;

                    default:
                    break;
                }
            }

        if (Alpha1)
        {
            Sheath();
        }

        if (Alpha2)
        {
            Ready_Knife();
        }

        if (Alpha3 && pistol != null)
        {
            Ready_Pistol();
        }

        if (Alpha4 && rifle != null)
        {
            Ready_Rifle();
        }

        switch (weaponEquipped)
        {
            case 0:
                Unarmed();
            break;
            case 1:
                if (knife != null)
                {
                    armsAnim.SetInteger("weapon", weaponEquipped);
                    UseKnife();
                }
            break;
            case 2:
                if (pistol != null)
                {
                    armsAnim.SetInteger("weapon", weaponEquipped);
                    UsePistol();
                }
            break;
            case 3:
                if (rifle != null)
                {
                    armsAnim.SetInteger("weapon", weaponEquipped);
                    UseRifle();
                }
            break;

            default:
            break;
        }
    } // End of Equipped_Weapon

    void Sheath()
    {
        scroll_Int = 0;
            
        weaponEquipped = 0;

        rifleDelay = false;
    }

    void Ready_Knife()
    {
        if (knife != null)
        {
            weaponEquipped = 1;
            Arms.SetActive(true);

            knifeEquip.Play();

            armsAnim.Play("swapToKnife");
        }
        
        rifleDelay = false;
    }

    void Ready_Pistol()
    {
        if (pistol != null)
        {
            weaponEquipped = 2;
            Arms.SetActive(true);

            pistolEquip.Play();

            armsAnim.Play("swapToPistol");
        }
        
        rifleDelay = false;
    }

    void Ready_Rifle()
    {
        if (rifle != null)
        {
            scroll_Int = 3;

            weaponEquipped = 3;
            Arms.SetActive(true);

            rifleEquip.Play();

            armsAnim.Play("swapToRifle");
        }
        
        rifleDelay = false;
    }

//====================================================================================

    void PrisonerArmed() // 7 => Player : 11 => PlayerPrisoner [Layer Ints]
    {
        gameObject.layer = 7;
    }

    void UseKnife()
    {
        Arms.SetActive(true);

        if (pistol != null)
        {
            pistol.SetActive(false);
        }

        if (knife != null)
        {
            knife.SetActive(true);
        }

        if (rifle != null)
        {
            rifle.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name == "WorkPrisonInside")
        {
            PrisonerArmed();
        }
        
        ResetMagPositions();
        
        AmmoCountDisplay.text = "Knife";

//=====================================================================================================
//=====================================================================================================

        if (!rt_use && Input.GetAxis("rightTrigger") > 0.75f)
        {
            rt_use = true;
            attackOnce = true;
        }

        if (Input.GetAxis("rightTrigger") < 0.5f)
        {
            rt_use = false;
        }

        if (Input.GetButtonDown("Fire1") || attackOnce)
        {
            attackOnce = false;

            knifeSwoop.Play();

            armsAnim.Play("stab");

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f))
            {
                if (hit.transform.GetComponent<LimbInfo>() != null)
                {
                    // enemy damaged
                    if (hit.transform.name == "head")
                    {
                        critMarker.Play("mark");
                    } else
                        {
                            hitMarker.Play("mark");
                        }

                    hit.transform.GetComponent<LimbInfo>().LimbInjured();
                }
            }
        }
    }

    void UsePistol()
    {
        Arms.SetActive(true);

        if (pistol != null)
        {
            pistol.SetActive(true);
        }

        if (knife != null)
        {
            knife.SetActive(false);
        }

        if (rifle != null)
        {
            rifle.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name == "WorkPrisonInside")
        {
            PrisonerArmed();
        }
        
        ResetMagPositions();

//=====================================================================================================
//=====================================================================================================

        if (!rt_use && Input.GetAxis("rightTrigger") > 0.75f)
        {
            rt_use = true;
            attackOnce = true;
        }

        if (Input.GetAxis("rightTrigger") < 0.5f)
        {
            rt_use = false;
        }

        if (!reloading && (Input.GetButtonDown("Fire1") || attackOnce) && pistolMagCount > 0)
        {
            attackOnce = false;
            armsAnim.Play("shootPistol");

            pistolShot.Play();

            pistol.transform.GetComponent<Animator>().Play("pistolSlideAnim");
            pistolFlash.Play();

            pistolMagCount--;

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 20))
            {
                if (hit.transform.name != transform.name)
                {
                    GameObject bulletHole = Instantiate(bulletHolePref, hit.point + hit.normal * 0.001f, Quaternion.identity);
                    bulletHole.transform.LookAt(hit.point + hit.normal);
                    bulletHole.transform.parent = hit.transform;
                    Destroy(bulletHole, 5f);

                    if (hit.transform.GetComponent<LimbInfo>() != null)
                    {
                        // enemy damaged
                        if (hit.transform.name == "head")
                        {
                            critMarker.Play("mark");
                        } else
                            {
                                hitMarker.Play("mark");
                            }
                            
                        print("Has Limb Script");
                        hit.transform.GetComponent<LimbInfo>().LimbInjured();
                    }

                    if (hit.transform.GetComponent<MissileScript>() != null)
                    {
                        hit.transform.GetComponent<MissileScript>().BulletHitRocket(hit.point);
                    }
                }
            }
        }

        if ((Input.GetKeyDown(KeyCode. R) || Input.GetKeyDown("joystick button 2")) && pistolMagCount < 15)
        {
            reloading = true;

            armsAnim.Play("pistolReload");

            Invoke("WeaponReloaded", 1.3f);

            int missingBullets = 15 - pistolMagCount;

            if (totalPistolAmmo >= missingBullets) // total 15 : missing 2 ==> total 13 : missing 0
            {
                pistolMagCount += missingBullets;
                totalPistolAmmo -= missingBullets;
            } else if (totalPistolAmmo < missingBullets) // total 7 : missing 10 : mag 5 ==> total 0 : missing 3 : mag 12
                {
                    pistolMagCount += totalPistolAmmo;
                    totalPistolAmmo = 0;
                }
        }

        AmmoCountDisplay.text = pistolMagCount + "/" + totalPistolAmmo;
    }

    void UseRifle()
    {
        Arms.SetActive(true);

        if (pistol != null)
        {
            pistol.SetActive(false);
        }

        if (knife != null)
        {
            knife.SetActive(false);
        }

        if (rifle != null)
        {
            rifle.SetActive(true);
        }

        if (SceneManager.GetActiveScene().name == "WorkPrisonInside")
        {
            PrisonerArmed();
        }
        
        ResetMagPositions();

        if ((Input.GetButton("Fire1") || Input.GetAxis("rightTrigger") > 0.75f) && !reloading && rifleMagCount > 0)
        {
            armsAnim.Play("shootRifle");

            if (produceShotSound)
            {
                StartCoroutine(rifleShotSoundRate(0.1f));
            }

            rifle.transform.GetComponent<Animator>().Play("rifleBlowBack");

            AlertFromLoudGunShot();
            
            if (!rifleDelay)
            {
                StartCoroutine(RateOfFireRifle());
            }
        }

        if ((Input.GetKeyDown(KeyCode. R) || Input.GetKeyDown("joystick button 2")) && rifleMagCount < 35)
        {
            reloading = true;

            armsAnim.Play("rifleReload");

            Invoke("WeaponReloaded", 1.3f);

            int missingBullets = 35 - rifleMagCount;

            if (totalRifleAmmo >= missingBullets) // total 15 : missing 2 ==> total 13 : missing 0
            {
                rifleMagCount += missingBullets;
                totalRifleAmmo -= missingBullets;
            } else if (totalRifleAmmo < missingBullets) // total 7 : missing 10 : mag 5 ==> total 0 : missing 3 : mag 12
                {
                    rifleMagCount += totalRifleAmmo;
                    totalRifleAmmo = 0;
                }
        }

        AmmoCountDisplay.text = rifleMagCount + "/" + totalRifleAmmo;
    }

    void AlertFromLoudGunShot()
    {
        if (GameObject.Find("BotNetAlert") != null)
        {
            BotNetAlert = GameObject.Find("BotNetAlert").GetComponent<LevelWideAlertness>();

            if (BotNetAlert != null)
            {
                if (BotNetAlert.enabled)
                {
                    if (!globalAlert)
                    {
                        BotNetAlert.AlertAllBots();
                        globalAlert = true;
                    }
                }
            }
        }
    }

    IEnumerator rifleShotSoundRate(float t)
    {
        produceShotSound = false;
        rifleShot.Play();

        yield return new WaitForSeconds(t);
        produceShotSound = true;
    }

    IEnumerator RateOfFireRifle()
    {
        rifleDelay = true;

        rifleMagCount--;

        yield return new WaitForSeconds(0.1f);

        pistolFlash.Play();

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 20))
        {
            if (hit.transform.name != transform.name)
            {
                GameObject bulletHole = Instantiate(bulletHolePref, hit.point + hit.normal * 0.001f, Quaternion.identity);
                bulletHole.transform.LookAt(hit.point + hit.normal);
                bulletHole.transform.parent = hit.transform;
                Destroy(bulletHole, 5f);

                if (hit.transform.GetComponent<LimbInfo>() != null)
                {
                    // enemy damaged
                    print(hit.transform.name);

                    if (hit.transform.GetComponent<LimbInfo>() != null)
                    {
                        // enemy damaged
                        if (hit.transform.name == "head")
                        {
                            critMarker.Play("mark");
                        } else
                            {
                                hitMarker.Play("mark");
                            }

                        hit.transform.GetComponent<LimbInfo>().LimbInjured();
                    }

                    hit.transform.GetComponent<LimbInfo>().LimbInjured();
                }

                if (hit.transform.GetComponent<MissileScript>() != null)
                {
                    hit.transform.GetComponent<MissileScript>().BulletHitRocket(hit.point);
                }
            }
        }

        rifleDelay = false;
    }

    void ResetMagPositions()
    {
        if (reloadAssist != null)
        {
            reloadAssist.PlacePistolMag();
            reloadAssist.PlaceRifleMag();
        }
    }

    void Unarmed()
    {
        Arms.SetActive(false);

        if (pistol != null)
        {
            pistol.SetActive(false);
        }

        if (knife != null)
        {
            knife.SetActive(false);
        }

        if (rifle != null)
        {
            rifle.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name == "WorkPrisonInside")
        {
            gameObject.layer = 11;
        }

        ResetMagPositions();
        AmmoCountDisplay.text = "Unarmed";
    }

//=================================================================================================================//

    public void AddAmmo(int pistolRounds, int rifleRounds)
    {
        totalPistolAmmo += pistolRounds;
        totalRifleAmmo += rifleRounds;
    }

    void WeaponReloaded()
    {
        reloading = false;
    }

//===============================================================//
        // raycast hit interaction methods
    public void AquireKnife()
    {
        knife = KnifeOnPlayer;
        knife.SetActive(true);
        GameObject.Find("PlayerWeaponsTracker").GetComponent<WeaponsTracker>().LinkKnife(knife);
    }

    public void AquirePrisonKnife(GameObject prisonKnife)
    {
        knife = prisonKnife;
        knife.SetActive(true);
        GameObject.Find("PlayerWeaponsTracker").GetComponent<WeaponsTracker>().LinkKnife(knife);
    }

    public void AquirePistol()
    {
        pistol = PistolOnPlayer;
        pistol.SetActive(true);
        GameObject.Find("PlayerWeaponsTracker").GetComponent<WeaponsTracker>().LinkPistol(pistol);
        PlayerPrefs.SetInt("HasPistol", 1);
    }

    public void AquireRifle()
    {
        rifle = RifleOnPlayer;
        rifle.SetActive(true);
        GameObject.Find("PlayerWeaponsTracker").GetComponent<WeaponsTracker>().LinkRifle(rifle);
        rifleDelay = false;
        PlayerPrefs.SetInt("HasRifle", 1);
    }

    public void LoadKeptWeapons(GameObject knf, GameObject pist, GameObject rif)
    {
        knife = knf;
        pistol = pist;
        rifle = rif;
    }
}//EndScript