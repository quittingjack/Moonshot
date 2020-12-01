using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public SpriteRenderer sr;
    public Color endPhaseColor;
    public AudioSource endPhaseSound;

    public float phase2HP;
    public float phase3HP;
    public float changePhaseInvincible;

    public GameObject launchPad;

    public AudioSource laser1SE;
    public AudioSource laser2SE;
    public AudioSource laser3SE;
    public AudioSource loopSE;
    public AudioSource explosionSE;

    public ScaleUp haloEffect;

    public Cinemachine.CinemachineImpulseSource moonShake;
    public Cinemachine.CinemachineImpulseSource moonShakeEnd;
    public Cinemachine.CinemachineImpulseSource moonShakeLoop;

    public WhiteScreen whiteScreen;

    [Header("Aiming")]
    public float facePlayerAngleSpeed;

    [Header("Normal Pattern")]
    public GameObject normalBullet;
    public float normalFireFreq;
    public float normalBulletSpeed;
    public float normalBulletIncreaseSpeed;

    float lastnormalFire;

    [Header("Normal Move")]

    public float normalMoveSpeed;
    public float recenterSpeed;

    [Header("Edge Move")]
    public Transform[] edgeMovePositions;
    public int[] edgeMoveSequence = { 0, 3, 1, 2, 0, 1, 3, 2 };
    public float edgeMoveSpeed;

    int currentEdgeMoveIndex;

    [Header("Rotate Pattern")]

    public GameObject rotateBullet;
    public float rotateFireFreq;
    public float rotateBulletSpeed;
    public float rotateBulletIncreaseSpeed;

    float lastRotateFire;

    [Header("Vortex Pattern")]
    public GameObject vortexBullet1;
    public GameObject vortexBullet2;
    public float vortexFireFreq;
    public float vortexBulletSpeed;
    public float vortexBulletIncreaseSpeed;

    float lastVortexFire;

    [Header("Random Pattern")]
    public GameObject randomBullet;
    public float randomFireFreq;
    public float randomBulletSpeed;
    public float randomBulletIncreaseSpeed;
    public int randomFireCount;
    public float randomFireDistance;

    [Header("qRotate Pattern")]

    public float qrotateFireFreq;
    public float qrotateBulletSpeed;
    public float qrotateBulletIncreaseSpeed;

    float lastqRotateFire;

    float lastRandomFire;

    [Header("Phase")]
    public float prepareTime;


    [Header("Manual Reference")]
    [SerializeField] GameObject normalPattern;
    [SerializeField] GameObject vortexPattern;
    [SerializeField] GameObject rotatePattern1;
    [SerializeField] GameObject rotatePattern2;

    [SerializeField] GameObject quickRotatePattern1;
    [SerializeField] GameObject quickRotatePattern2;
    [SerializeField] MoveBoundary normalMoveBound;

    [Header("Auto Reference")]
    [SerializeField] GameObject player;
    [SerializeField] FightStatus fightStatus;
    [SerializeField] int currentPhase;
    [SerializeField] float startTime;
    [SerializeField] float changePhaseTime;
    [SerializeField] float actionTime;




    Vector3 normalMoveGoal;


    private void Awake()
    {
        lastnormalFire = -normalFireFreq;
        lastRotateFire = -rotateFireFreq;

        player = GameObject.FindGameObjectWithTag("Player");

        fightStatus = GetComponent<FightStatus>();

        normalMoveGoal = normalMoveBound.GetRandomPosInBound();
    }

    private void Start()
    {
        startTime = Time.time;
    }
    private void FixedUpdate()
    {
        

        if (currentPhase == 0)
        {
            
            fightStatus.isInvincible = true;
            Aiming();
            if (Time.time - startTime > prepareTime)
            {
                currentPhase = 1;
                changePhaseTime = Time.time;
                actionTime = Time.time;
            }
        }
        else if (currentPhase == 1)
        {
            fightStatus.isInvincible = false;
            Aiming();
            float actionTimeDiff = Time.time - actionTime;
            // phase 1.action 1
            if (actionTimeDiff < 15)
            {
                bool reach = MoveToTarget(normalMoveSpeed, normalMoveGoal);

                if (reach)
                {
                    normalMoveGoal = normalMoveBound.GetRandomPosInBound();
                }
                
                bool fired = FixedPatternFire(ref lastnormalFire, normalFireFreq, normalPattern, normalBullet, normalBulletSpeed, normalBulletIncreaseSpeed);
                if (fired)
                {
                    laser3SE.Play();
                }
            }
            // phase 1.action 2
            else if (actionTimeDiff >= 15 && actionTimeDiff < 18)
            {
                bool reach = MoveToTarget(recenterSpeed, Vector3.zero);
            }
            // phase 1.action 3
            else if (actionTimeDiff >= 18 && actionTimeDiff < 28)
            {
                bool fired = FixedPatternFire(ref lastRotateFire, rotateFireFreq, rotatePattern1, rotateBullet, rotateBulletSpeed, rotateBulletIncreaseSpeed);
                if (fired)
                {
                    lastRotateFire = -100; // reset fire time
                    FixedPatternFire(ref lastRotateFire, rotateFireFreq, rotatePattern2, rotateBullet, rotateBulletSpeed, rotateBulletIncreaseSpeed);

                    if (fired)
                    {
                        laser2SE.Play();
                    }

                }
            }
            else if (actionTimeDiff >= 28 && actionTimeDiff < 31)
            {
            }
            // reset phase 1
            else
            {
                actionTime = Time.time;
            }

            if (fightStatus.getHP() <= phase2HP)
            {
                changePhaseTime = Time.time;
                actionTime = Time.time;
                currentPhase = 2;
                explosionSE.Play();
                haloEffect.StartScaleUp();
                moonShake.GenerateImpulse();
                fightStatus.isInvincible = true;
            }
        }
        else if (currentPhase == 2)
        {
            Aiming();
            float actionTimeDiff = Time.time - actionTime;

            if (actionTimeDiff > changePhaseInvincible)
                fightStatus.isInvincible = false;

            if (actionTimeDiff < 3)
            {
                bool reach = MoveToTarget(recenterSpeed, Vector3.zero);
            }
            else if (actionTimeDiff >= 3 && actionTimeDiff < 13)
            {
                RandomFire(ref lastRandomFire, randomFireFreq, randomBullet, randomBulletSpeed, randomBulletIncreaseSpeed, randomFireCount, randomFireDistance, transform.position);
                if (!loopSE.isPlaying) loopSE.Play();
            }
            else if (actionTimeDiff >= 13 && actionTimeDiff < 19)
            {
                RandomFire(ref lastRandomFire, randomFireFreq, randomBullet, randomBulletSpeed, randomBulletIncreaseSpeed, randomFireCount, randomFireDistance, transform.position);
                bool fired = FixedPatternFire(ref lastVortexFire, vortexFireFreq, vortexPattern, vortexBullet1, vortexBulletSpeed, vortexBulletIncreaseSpeed);
                if (fired)
                {
                    laser1SE.Play();
                }
            }
            else if (actionTimeDiff >= 19 && actionTimeDiff < 25)
            {
                RandomFire(ref lastRandomFire, randomFireFreq, randomBullet, randomBulletSpeed, randomBulletIncreaseSpeed, randomFireCount, randomFireDistance, transform.position);
                bool fired = FixedPatternFire(ref lastVortexFire, vortexFireFreq, vortexPattern, vortexBullet2, vortexBulletSpeed, vortexBulletIncreaseSpeed);
                if (fired)
                {
                    laser1SE.Play();
                }
            }
            else
            {
                if (loopSE.isPlaying) loopSE.Stop();
                actionTime = Time.time;
            }

            if (fightStatus.getHP() <= phase3HP)
            {
                if (loopSE.isPlaying) loopSE.Stop();
                changePhaseTime = Time.time;
                actionTime = Time.time;
                currentPhase = 3;
                currentEdgeMoveIndex = 0;
                explosionSE.Play();
                haloEffect.StartScaleUp();
                moonShake.GenerateImpulse();
                
                fightStatus.isInvincible = true;
            }
        }
        else if (currentPhase == 3)
        {
            float actionTimeDiff = Time.time - actionTime;
            Aiming();
            if (actionTimeDiff > changePhaseInvincible)
                fightStatus.isInvincible = false;

            if (actionTimeDiff <= 3)
            {
                // pause 3s
            }
            else if (actionTimeDiff >= 3 && actionTimeDiff < 13)
            {
                bool reach = MoveToTarget(edgeMoveSpeed, edgeMovePositions[currentEdgeMoveIndex].position);

                if (reach)
                {
                    currentEdgeMoveIndex = (currentEdgeMoveIndex + 1) % edgeMovePositions.Length;
                }

                bool fired = FixedPatternFire(ref lastqRotateFire, qrotateFireFreq, quickRotatePattern1, rotateBullet, qrotateBulletSpeed, qrotateBulletIncreaseSpeed);
                if (fired)
                {
                    laser2SE.Play();
                }
            }
            else if (actionTimeDiff >= 13 && actionTimeDiff < 16)
            {
                // pause 3s
            }
            else if (actionTimeDiff >= 16 && actionTimeDiff < 26)
            {
                bool fired = FixedPatternFire(ref lastqRotateFire, qrotateFireFreq, quickRotatePattern1, rotateBullet, qrotateBulletSpeed, qrotateBulletIncreaseSpeed);
                if (fired)
                {
                    laser2SE.Play();
                }
                fired = FixedPatternFire(ref lastVortexFire, vortexFireFreq, vortexPattern, vortexBullet2, vortexBulletSpeed, vortexBulletIncreaseSpeed);
                if (fired)
                {
                    laser1SE.Play();
                }

            }
            else if (actionTimeDiff >= 26 && actionTimeDiff < 29)
            {
                // pause 3s
            }
            else if (actionTimeDiff >= 29 && actionTimeDiff < 39)
            {
                bool reach = MoveToTarget(edgeMoveSpeed, edgeMovePositions[currentEdgeMoveIndex].position);

                if (reach)
                {
                    currentEdgeMoveIndex = (currentEdgeMoveIndex + 1) % edgeMovePositions.Length;
                }

                bool fired = FixedPatternFire(ref lastRotateFire, rotateFireFreq, quickRotatePattern2, rotateBullet, rotateBulletSpeed, rotateBulletIncreaseSpeed);
                if (fired)
                {
                    laser2SE.Play();
                }
            }
            else if (actionTimeDiff >= 39 && actionTimeDiff < 42)
            {
                // pause 3s
            }
            else if (actionTimeDiff >= 42 && actionTimeDiff < 52)
            {
                bool fired = FixedPatternFire(ref lastqRotateFire, qrotateFireFreq, quickRotatePattern2, rotateBullet, qrotateBulletSpeed, qrotateBulletIncreaseSpeed);
                if (fired)
                {
                    laser2SE.Play();
                }
                fired = FixedPatternFire(ref lastVortexFire, vortexFireFreq, vortexPattern, vortexBullet1, vortexBulletSpeed, vortexBulletIncreaseSpeed);
                if (fired)
                {
                    laser1SE.Play();
                }
            }
            else
            {
                actionTime = Time.time;
            }

            if (fightStatus.getHP() <= 0)
            {
                changePhaseTime = Time.time;
                actionTime = Time.time;
                currentPhase = 4;
                currentEdgeMoveIndex = 0;
                explosionSE.Play();
                haloEffect.StartScaleUp();
                moonShake.GenerateImpulse();
                endPhaseSound.Play();

            }
        }
        else if (currentPhase == 4)
        {
            float actionTimeDiff = Time.time - actionTime;
            AimingPhase4();
            if (actionTimeDiff < 3)
            {
                bool reach = MoveToTarget(recenterSpeed, Vector3.zero);
            }
            else if (actionTimeDiff >= 3 && actionTimeDiff < 18)
            {
                if (!loopSE.isPlaying) loopSE.Play();
                RandomFire(ref lastRandomFire, randomFireFreq, randomBullet, randomBulletSpeed, randomBulletIncreaseSpeed, randomFireCount, randomFireDistance, transform.position);
            }
            else if (actionTimeDiff >= 18 && actionTimeDiff < 21)
            {
                RandomFire(ref lastRandomFire, randomFireFreq, randomBullet, randomBulletSpeed, randomBulletIncreaseSpeed, randomFireCount, randomFireDistance, transform.position);
            }
            else if (actionTimeDiff >= 21 && actionTimeDiff < 36)
            {
                RandomFire(ref lastRandomFire, randomFireFreq, randomBullet, randomBulletSpeed, randomBulletIncreaseSpeed, randomFireCount - 1, randomFireDistance, transform.position);
                bool fired = FixedPatternFire(ref lastnormalFire, normalFireFreq * 1.3f, normalPattern, normalBullet, normalBulletSpeed, normalBulletIncreaseSpeed);
                if (fired)
                {
                    laser3SE.Play();
                }
            }
            else if (actionTimeDiff >= 36 && actionTimeDiff < 39)
            {
                RandomFire(ref lastRandomFire, randomFireFreq, randomBullet, randomBulletSpeed, randomBulletIncreaseSpeed, randomFireCount - 1, randomFireDistance, transform.position);
                bool fired = FixedPatternFire(ref lastnormalFire, normalFireFreq * 1.3f, normalPattern, normalBullet, normalBulletSpeed, normalBulletIncreaseSpeed);
                if (fired)
                {
                    laser3SE.Play();
                }
            }
            else if (actionTimeDiff >= 39 && actionTimeDiff < 49)
            {
                RandomFire(ref lastRandomFire, randomFireFreq, randomBullet, randomBulletSpeed, randomBulletIncreaseSpeed, randomFireCount - 2, randomFireDistance, transform.position);
                bool fired = FixedPatternFire(ref lastnormalFire, normalFireFreq * 1.3f, normalPattern, normalBullet, normalBulletSpeed, normalBulletIncreaseSpeed);
                if (fired)
                {
                    laser3SE.Play();
                }
                fired = FixedPatternFire(ref lastVortexFire, vortexFireFreq*1.6f, vortexPattern, vortexBullet1, vortexBulletSpeed, vortexBulletIncreaseSpeed);
                if (fired)
                {
                    laser1SE.Play();
                }
            }
            else if (actionTimeDiff >= 49 && actionTimeDiff < 51)
            {
                RandomFire(ref lastRandomFire, randomFireFreq, randomBullet, randomBulletSpeed, randomBulletIncreaseSpeed, randomFireCount - 2, randomFireDistance, transform.position);
                bool fired = FixedPatternFire(ref lastnormalFire, normalFireFreq * 1.3f, normalPattern, normalBullet, normalBulletSpeed, normalBulletIncreaseSpeed);
                if (fired)
                {
                    laser3SE.Play();
                }
                fired = FixedPatternFire(ref lastVortexFire, vortexFireFreq * 1.6f, vortexPattern, vortexBullet1, vortexBulletSpeed, vortexBulletIncreaseSpeed);
                if (fired)
                {
                    laser1SE.Play();
                }
            }
            else if (actionTimeDiff >= 51 && actionTimeDiff < 58)
            {
                RandomFire(ref lastRandomFire, randomFireFreq, randomBullet, randomBulletSpeed, randomBulletIncreaseSpeed, randomFireCount - 2, randomFireDistance, transform.position);
                bool fired = FixedPatternFire(ref lastnormalFire, normalFireFreq * 1.3f, normalPattern, normalBullet, normalBulletSpeed, normalBulletIncreaseSpeed);
                if (fired)
                {
                    laser3SE.Play();
                }
                fired = FixedPatternFire(ref lastVortexFire, vortexFireFreq * 1.6f, vortexPattern, vortexBullet1, vortexBulletSpeed, vortexBulletIncreaseSpeed);
                if (fired)
                {
                    laser1SE.Play();
                }
                fired = FixedPatternFire(ref lastRotateFire, rotateFireFreq * 1.3f, rotatePattern2, rotateBullet, rotateBulletSpeed * 0.6f, rotateBulletIncreaseSpeed);
                if (fired)
                {
                    laser2SE.Play();
                }
            }

            if (actionTimeDiff < 58)
            {
                moonShakeLoop.GenerateImpulse();
            }

            sr.color = Color.Lerp(Color.white, endPhaseColor, actionTimeDiff / 58.0f);

            if (actionTimeDiff >= 58)
            {
                if (loopSE.isPlaying) loopSE.Stop();
                changePhaseTime = Time.time;
                actionTime = Time.time;
                currentEdgeMoveIndex = 0;
                explosionSE.Play();
                currentPhase = 5;
                haloEffect.StartScaleUp();
                moonShakeEnd.GenerateImpulse();
                whiteScreen.gameObject.SetActive(true);
                player.GetComponent<FightStatus>().isInvincible = true;
                
            }
        }
        else if (currentPhase == 5)
        {
            float actionTimeDiff = Time.time - actionTime;
            if (actionTimeDiff > 5)
            {
                transform.rotation = Quaternion.identity;
                launchPad.SetActive(true);
                sr.color = Color.white;
                endPhaseSound.Stop();
            }

        }
    }

    bool MoveToTarget(float speed, Vector3 goal)
    {
        float move = speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, goal) < move)
        {
            transform.position = goal;
            return true;
        }
        else
        {
            transform.position += (goal - transform.position).normalized * move;
            return false;
        }
        
    }
    bool FixedPatternFire(ref float lastFire, float freq, GameObject pattern, GameObject prefab, float speed, float increaseSpeed)
    {
        if (CanFire(lastFire, freq))
        {
            lastFire = Time.time;

            for (int i = 0; i < pattern.transform.childCount; i++)
            {
                Transform t = pattern.transform.GetChild(i);
                SpawnBullet(t.position, t.up, prefab, speed, increaseSpeed);
            }
            return true;
        }
        return false;
    }
    bool RandomFire(ref float lastFire, float freq, GameObject prefab, float speed, float increaseSpeed, int fireCount, float distance, Vector3 center)
    {
        if (CanFire(lastFire, freq))
        {
            lastFire = Time.time;

            for (int i = 0; i < fireCount; i++)
            {
                Vector3 dir = Random.insideUnitCircle.normalized;
                SpawnBullet(center + dir * distance, dir, prefab, speed, increaseSpeed);
            }
            return true;
        }
        return false;
    }
    void SpawnBullet(Vector3 pos, Vector3 dir, GameObject prefab, float speed, float increaseSpeed)
    {
        Bullet bullet = Instantiate(prefab, pos, Quaternion.identity).GetComponent<Bullet>();
        bullet.damage = fightStatus.power;
        bullet.src = BulletSource.enemy;
        bullet.spawnSource = gameObject;
        bullet.speed = speed;
        bullet.direction = dir;
        bullet.increaseVelOverTime = increaseSpeed;


        Destroy(bullet.gameObject, 15.0f);
    }

    void Aiming()
    {
        Quaternion face = Quaternion.LookRotation(Vector3.forward, player.transform.position - transform.position);
        Quaternion curr = transform.rotation;
        float diff = Quaternion.Angle(face, curr);
        float rotate = facePlayerAngleSpeed * Time.deltaTime;
        if (diff < rotate)
        {
            transform.rotation = face;
        }
        else
        {
            transform.rotation = Quaternion.Lerp(curr, face, rotate / diff);
        }
    }

    void AimingPhase4()
    {
        Quaternion face = Quaternion.LookRotation(Vector3.forward, player.transform.position - transform.position);
        Quaternion curr = transform.rotation;
        float diff = Quaternion.Angle(face, curr);
        float rotate = facePlayerAngleSpeed * 0.5f * Time.deltaTime;
        if (diff < rotate)
        {
            transform.rotation = face;
        }
        else
        {
            transform.rotation = Quaternion.Lerp(curr, face, rotate / diff);
        }
    }

    bool CanFire(float lastTime, float frequence)
    {
        return Time.time - lastTime > frequence;
    }
}
