using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceControl : MonoBehaviour
{
    public GameObject winText;
    public GameObject loseText;
    public AudioSource loseSE;
    public GameObject loseEffect;
    public GameObject spriteObj;

    [Header("Damage")]
    public int frontGunDamage;
    public int sideGunDamage;
    public int backGunDamage;
    public int mainGunDamage;

    [Header("Reload Setting")]
    public int mainGunSlot;
    public int sideGunSlot;
    public int frontGunSlot;

    public float mainGunReloadTime;
    public float sideGunReloadTime;
    public float frontGunReloadTime;


    public float backGunMaxHeat;
    public float backGunHeatIncrease;
    public float backGunHeatDecrease;
    public float backGunOverHeatDecrease;
    public SpriteRenderer backGunSprite;
    public Color backGunNormalColor;
    public Color backGunHeatColor;
    public float backGunSoundPitchMaxHeat;
    public float backGunSoundPitchMinHeat;

    public float sideGunSoundPitchMax;
    public float sideGunSoundPitchMin;
    public float mainGunSoundPitchMax;
    public float mainGunSoundPitchMin;
    public float frontGunSoundPitchMax;
    public float frontGunSoundPitchMin;

    public float backGunHeat;
    public bool backGunOverHeat;
    public int mainGunMag;
    public int leftGunMag;
    public int rightGunMag;
    public int frontGunMag;

    public float lastMainGunReload;
    public float lastLeftGunReload;
    public float lastRightGunReload;
    public float lastFrontGunReload;

    public float manualReloadDoubleTapTime;
    public float manualReloadCD;
    public float lastManualReloadTap;
    public float lastManualReload;


    [Header("Sound Effect")]
    public AudioSource mainGunSE;
    public AudioSource sideGunSE;
    public AudioSource frontGunSE;
    public FadeInOutAudioSource backGunSE;
    public AudioSource backGunOverheatSE;
    public AudioSource backGunOverheatAlarmSE;
    public float backGunFadeInTime;
    public float backGunFadeOutTime;
    public AudioSource emptySE;
    public AudioSource manualReloadSE;


    [Header("Force Power")]
    public float backForce;
    public float midForce;
    public float frontForce;
    public float mainForce;

    public float backgunForceIncreaseOvertime;
    public float backgunForceIncreaseOvertimeMax;
    public float backgunForceDecreaseOvertime;

    [Header("Bullet Speed")]
    public float frontSpeed;
    public float backSpeed;
    public float sideSpeed;
    public float mainSpeed;

    [Header("Force Position")]
    public Transform LBack;
    public Transform RBack;
    public Transform LMid;
    public Transform RMid;
    public Transform LFront;
    public Transform RFront;
    public Transform MainFire;

    [Header("Fire Frequence")]
    public float backFireFreq;
    public float frontFireFreq;
    public float leftFireFreq;
    public float rightFireFreq;
    public float mainFireFreq;

    [Header("Manual Reference")]
    [SerializeField] GameObject mainGun;
    [SerializeField] ParticleSystem[] mainGunEffects;

    [SerializeField] ColliderDelegate landingCollider;
    [SerializeField] GameObject boundaryDefine;

    [SerializeField] Cinemachine.CinemachineImpulseSource frontShake;
    [SerializeField] Cinemachine.CinemachineImpulseSource backShake;
    [SerializeField] Cinemachine.CinemachineImpulseSource mainShake;
    [SerializeField] Cinemachine.CinemachineImpulseSource sideShake;

    [Header("Auto Reference")]
    [SerializeField] BulletFire bulletFire;
    [SerializeField] Transform L_soft;
    [SerializeField] Transform L_hard;
    [SerializeField] Transform R_soft;
    [SerializeField] Transform R_hard;
    [SerializeField] Transform U_soft;
    [SerializeField] Transform U_hard;
    [SerializeField] Transform D_soft;
    [SerializeField] Transform D_hard;

    Rigidbody2D rb;

    float lastFireBack;
    float lastFireFront;
    float lastFireLeft;
    float lastFireRight;
    float lastFireMain;
    float currentBackForceIncrease;

    float startTime;
    bool control;

    public void Death()
    {
        if (control)
        {
            loseText.SetActive(true);
            control = false;
            loseSE.Play();
            GetComponent<Collider2D>().enabled = false;
            GameObject obj = Instantiate(loseEffect);
            obj.transform.position = transform.position;
            spriteObj.SetActive(false);
        }
    }

    public void OnTouchLandingCollider(Collider2D collision)
    {
        if (collision.tag == "LandingBox")
        {
            transform.position = collision.transform.GetChild(0).position;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.MoveRotation(0);
            winText.SetActive(true);
            winText.GetComponent<WinText>().SetTime(Time.time - startTime);
            control = false;
            backGunSE.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        control = true;
        rb = GetComponent<Rigidbody2D>();
        bulletFire = GetComponent<BulletFire>();
        lastFireBack = 0;
        lastFireFront = 0;
        lastFireLeft = 0;
        lastFireRight = 0;
        lastFireMain = 0;

        L_soft = boundaryDefine.transform.Find("L_Soft");
        L_hard = boundaryDefine.transform.Find("L_Hard");
        R_soft = boundaryDefine.transform.Find("R_Soft");
        R_hard = boundaryDefine.transform.Find("R_Hard");
        D_soft = boundaryDefine.transform.Find("D_Soft");
        D_hard = boundaryDefine.transform.Find("D_Hard");
        U_soft = boundaryDefine.transform.Find("U_Soft");
        U_hard = boundaryDefine.transform.Find("U_Hard");

        backGunHeat = 0;

        lastMainGunReload = -mainGunReloadTime;
        lastLeftGunReload = -sideGunReloadTime;
        lastRightGunReload = -sideGunReloadTime;
        lastFrontGunReload = -frontGunReloadTime;

        mainGunMag = mainGunSlot;
        leftGunMag = sideGunSlot;
        rightGunMag = sideGunSlot;
        frontGunMag = frontGunSlot;

        lastManualReload = -manualReloadCD;
    }
    
    void Start()
    {
        landingCollider.EventOnTriggerEnter2D += OnTouchLandingCollider;
        startTime = Time.time;
    }


    private void FixedUpdate()
    {
        if (!control)
            return;
        bool emptySouldPlay = false;

        if (Input.GetKey(KeyCode.W) && !backGunOverHeat)
        {
            if (CanFire(lastFireBack, backFireFreq))
            {
                rb.AddForceAtPosition(transform.up * (backForce + currentBackForceIncrease), LBack.position, ForceMode2D.Impulse);
                rb.AddForceAtPosition(transform.up * (backForce + currentBackForceIncrease), RBack.position, ForceMode2D.Impulse);
                lastFireBack = Time.time;

                bulletFire.FireMinigun(LBack.position, -transform.up, backSpeed, 0, backGunDamage);
                bulletFire.FireMinigun(RBack.position, -transform.up, backSpeed, 1, backGunDamage);
            }

            foreach (ParticleSystem ps in mainGunEffects)
            {
                if (!ps.isPlaying)
                    ps.Play();
            }

            backShake.GenerateImpulse(-transform.up);

            currentBackForceIncrease += backgunForceIncreaseOvertime * Time.fixedDeltaTime;
            if (currentBackForceIncrease > backgunForceIncreaseOvertimeMax)
                currentBackForceIncrease = backgunForceIncreaseOvertimeMax;

            if (backGunSE.audioSource.volume < 0.5 && backGunSE.fadeSpeed <= 0)
                backGunSE.FadeTo(backGunFadeInTime, 0.5f);

            backGunHeat += backGunHeatIncrease * Time.fixedDeltaTime;
            backGunSE.audioSource.pitch = Mathf.Lerp(backGunSoundPitchMinHeat, backGunSoundPitchMaxHeat, backGunHeat / backGunMaxHeat);
            if (backGunHeat > backGunMaxHeat)
            {
                backGunHeat = backGunMaxHeat;
                backGunOverHeat = true;
                backGunOverheatSE.Play();
                backGunOverheatAlarmSE.Play();
            }
        }
        else
        {

            if (backGunSE.audioSource.volume > 0 && backGunSE.fadeSpeed >= 0)
                backGunSE.FadeTo(backGunFadeOutTime, 0);

            foreach (ParticleSystem ps in mainGunEffects)
            {
                ps.Stop();
            }


            currentBackForceIncrease -= backgunForceDecreaseOvertime * Time.fixedDeltaTime;
            if (currentBackForceIncrease < 0)
                currentBackForceIncrease = 0;

            if (backGunOverHeat)
            {
                backGunHeat -= backGunOverHeatDecrease * Time.fixedDeltaTime;
                if (backGunHeat <= 0)
                {
                    backGunOverHeat = false;
                    backGunHeat = 0;
                }
            } 
            else
            {
                backGunHeat -= backGunHeatDecrease * Time.fixedDeltaTime;
                if (backGunHeat <= 0)
                    backGunHeat = 0;
            }
            

        }
        if (Input.GetKey(KeyCode.A))
        {
            if (CanFire(lastFireLeft, leftFireFreq) && Time.time - lastLeftGunReload > sideGunReloadTime && leftGunMag > 0)
            {
                rb.AddForceAtPosition(transform.right * midForce * Time.fixedDeltaTime, LMid.position, ForceMode2D.Impulse);
                lastFireLeft = Time.time;

                bulletFire.Fire(LMid.position, -transform.right, sideSpeed, BulletType.handgun, sideGunDamage);
                sideShake.GenerateImpulse();
                sideGunSE.pitch = Mathf.Lerp(sideGunSoundPitchMax, sideGunSoundPitchMin, (float)leftGunMag / sideGunSlot);
                sideGunSE.Play();

                leftGunMag -= 1;
                if (leftGunMag == 0)
                {
                    lastLeftGunReload = Time.time;
                    leftGunMag = sideGunSlot;
                }
            }
            else if (Time.time - lastLeftGunReload < sideGunReloadTime)
            {
                emptySouldPlay = true;
            }
            
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (CanFire(lastFireRight, rightFireFreq) && Time.time - lastRightGunReload > sideGunReloadTime && rightGunMag > 0)
            {
                rb.AddForceAtPosition(-transform.right * midForce * Time.fixedDeltaTime, RMid.position, ForceMode2D.Impulse);
                lastFireRight = Time.time;

                bulletFire.Fire(RMid.position, transform.right, sideSpeed, BulletType.handgun, sideGunDamage);
                sideShake.GenerateImpulse();
                sideGunSE.pitch = Mathf.Lerp(sideGunSoundPitchMax, sideGunSoundPitchMin, (float)rightGunMag / sideGunSlot);
                sideGunSE.Play();

                rightGunMag -= 1;
                if (rightGunMag == 0)
                {
                    lastRightGunReload = Time.time;
                    rightGunMag = sideGunSlot;
                }
            }
            else if (Time.time - lastRightGunReload < sideGunReloadTime)
            {
                emptySouldPlay = true;
            }

        }
        if (Input.GetKey(KeyCode.S) )
        {
            if (CanFire(lastFireFront, frontFireFreq) && Time.time - lastFrontGunReload > frontGunReloadTime && frontGunMag > 0)
            {
                rb.AddForceAtPosition(-transform.up * frontForce, LFront.position, ForceMode2D.Impulse);
                rb.AddForceAtPosition(-transform.up * frontForce, RFront.position, ForceMode2D.Impulse);
                lastFireFront = Time.time;

                bulletFire.Fire(LFront.position, transform.up, frontSpeed, BulletType.shotgun, frontGunDamage);
                bulletFire.Fire(RFront.position, transform.up, frontSpeed, BulletType.shotgun, frontGunDamage);

                frontShake.GenerateImpulse(transform.up);
                frontGunSE.pitch = Mathf.Lerp(frontGunSoundPitchMax, frontGunSoundPitchMin, (float)frontGunMag / frontGunSlot);
                frontGunSE.Play();

                frontGunMag -= 1;
                if (frontGunMag == 0)
                {
                    lastFrontGunReload = Time.time;
                    frontGunMag = frontGunSlot;
                }
            }
            else if ( Time.time - lastFrontGunReload < frontGunReloadTime)
            {
                emptySouldPlay = true;
            }

        }


        if (Input.GetMouseButton(0))
        {
            if (CanFire(lastFireMain, mainFireFreq) && Time.time - lastMainGunReload > mainGunReloadTime && mainGunMag > 0)
            {
                rb.AddForceAtPosition(-mainGun.transform.up * mainForce, MainFire.position, ForceMode2D.Impulse);
                lastFireMain = Time.time;

                bulletFire.Fire(MainFire.position, mainGun.transform.up, mainSpeed, BulletType.maingun, mainGunDamage);
                mainShake.GenerateImpulse();
                mainGunSE.pitch = Mathf.Lerp(mainGunSoundPitchMax, mainGunSoundPitchMin, (float)mainGunMag / mainGunSlot);
                mainGunSE.Play();

                mainGunMag -= 1;
                if (mainGunMag == 0)
                {
                    lastMainGunReload = Time.time;
                    mainGunMag = mainGunSlot;
                }
            }
            else if (Time.time - lastMainGunReload < mainGunReloadTime)
            {
                emptySouldPlay = true;
            }
        }

        if (emptySouldPlay && !emptySE.isPlaying)
        {
            emptySE.Play();
        }
        else if (!emptySouldPlay && emptySE.isPlaying)
        {
            emptySE.Stop();
        }

        BoundaryVelocityLimit();
    }

    // Update is called once per frame
    void Update()
    {
        if (!control)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadScene("Menu");
            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene("Main");
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePos.z = 0;

        mainGun.transform.rotation = Quaternion.LookRotation(Vector3.forward, (mousePos - transform.position).normalized);

        backGunSprite.color = Color.Lerp(backGunNormalColor, backGunHeatColor, backGunHeat / backGunMaxHeat);

        if (Input.GetKeyDown(KeyCode.R))
        {
            if ((Time.time - lastManualReloadTap < manualReloadDoubleTapTime) && (Time.time - lastManualReload > manualReloadCD)) ManualReload();
            lastManualReloadTap = Time.time;
        }
    }

    void ManualReload()
    {
        lastManualReload = lastFrontGunReload = lastLeftGunReload = lastRightGunReload = lastMainGunReload = Time.time;
        rightGunMag = sideGunSlot;
        frontGunMag = frontGunSlot;
        leftGunMag = sideGunSlot;
        mainGunMag = mainGunSlot;

        manualReloadSE.Play();
    }

    bool CanFire(float lastTime, float frequence)
    {
        return Time.time - lastTime > frequence;
    }

    void BoundaryVelocityLimit()
    {
        Vector3 pos = transform.position;
        Vector3 vel = rb.velocity;

        if (pos.x < L_soft.position.x)
        {
            if (pos.x < L_hard.position.x)
            {
                vel.x = 0;
            }
            else
            {
                float boundary = Mathf.Abs(L_hard.position.x - L_soft.position.x);
                float ratio = Mathf.Abs(L_soft.position.x - pos.x) / boundary;
                if (vel.x < 0)
                {
                    vel.x = Mathf.Lerp(vel.x, 0, ratio * ratio);
                }
            }
        }

        if (pos.x > R_soft.position.x)
        {
            if (pos.x > R_hard.position.x)
            {
                vel.x = 0;
            }
            else
            {
                float boundary = Mathf.Abs(R_hard.position.x - R_soft.position.x);
                float ratio = Mathf.Abs(R_soft.position.x - pos.x) / boundary;
                if (vel.x > 0)
                {
                    vel.x = Mathf.Lerp(vel.x, 0, ratio * ratio);
                }
            }
        }

        if (pos.y < D_soft.position.y)
        {
            if (pos.y < D_hard.position.y)
            {
                vel.y = 0;
            }
            else
            {
                float boundary = Mathf.Abs(D_hard.position.y - D_soft.position.y);
                float ratio = Mathf.Abs(D_soft.position.y - pos.y) / boundary;
                if (vel.y < 0)
                {
                    vel.y = Mathf.Lerp(vel.y, 0, ratio * ratio);
                }
            }
        }

        if (pos.y > U_soft.position.y)
        {
            if (pos.y > U_hard.position.y)
            {
                vel.y = 0;
            }
            else
            {
                float boundary = Mathf.Abs(U_hard.position.y - U_soft.position.y);
                float ratio = Mathf.Abs(U_soft.position.y - pos.y) / boundary;
                if (vel.y > 0)
                {
                    vel.y = Mathf.Lerp(vel.y, 0, ratio * ratio);
                }
            }
        }

        rb.velocity = vel;
    }

    
}
