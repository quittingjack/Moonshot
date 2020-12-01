using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    handgun = 0,
    minigun,
    shotgun,
    maingun
}

public class BulletFire : MonoBehaviour
{
    public BulletType bulletTypeIndex;

    public GameObject[] bulletPrefabs;
    public TrailRenderer[] bulletTrails;

    [Header("Effect Parameter")]
    [Range(0, 10.0f)]
    public float randomOffset;
    public float minigunTrailOffsetMin;
    public float minigunTrailOffsetMax;
    public float minigunTrailDistanceMin;
    public float minigunTrailDistanceMax;

    public float minigunTrailHitOffsetMinRatio;
    public float minigunTrailHitOffsetMaxRatio;
    public float minigunTrailHitDistanceMinRatio;
    public float minigunTrailHitDistanceMaxRatio;

    public float shotgunTrailOffsetMin;
    public float shotgunTrailOffsetMax;
    public float shotgunTrailDistanceMin;
    public float shotgunTrailDistanceMax;

    public float shotgunTrailHitOffsetMinRatio;
    public float shotgunTrailHitOffsetMaxRatio;
    public float shotgunTrailHitDistanceMinRatio;
    public float shotgunTrailHitDistanceMaxRatio;

    public ParticleSystem[] backgunHitEffects;
    public float backgunHitEffectDisappear;
    float[] lastBackgunHitEffect;

    public AudioSource[] backgunHitSE;

    public int mainGunBulletCountMin;
    public int mainGunBulletCountMax;
    public float mainGunSpreadAngleMin;
    public float mainGunSpreadAngleMax;
    public float mainGunBulletDurationMin;
    public float mainGunBulletDurationMax;
    public float mainGunBulletSpeedOffsetMin;
    public float mainGunBulletSpeedOffsetMax;

    [Header("Auto Reference")]
    [SerializeField] FightStatus fightStatus;
    [SerializeField] BulletSource sourceType;

    int bulletLayerMask;

    private void Awake()
    {
        fightStatus = GetComponent<FightStatus>();
        if (tag == "Player")
            sourceType = BulletSource.player;
        else
            sourceType = BulletSource.enemy;

        lastBackgunHitEffect = new float[backgunHitEffects.Length];
        bulletLayerMask = ~LayerMask.GetMask(new string[] { "Bullet" });
    }
    private void Update()
    {
        for (int i = 0; i < backgunHitEffects.Length; i++)
        {
            if (Time.time - lastBackgunHitEffect[i] > backgunHitEffectDisappear)
            {
                backgunHitEffects[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
    public void FireMinigun(Vector2 position, Vector2 direction, float speed, int gunIndex, int power)
    {
        BulletType bulletType = BulletType.minigun;
        // Create bullet trail
        TrailRenderer trail = Instantiate(bulletTrails[(int)bulletType], position, Quaternion.identity);

        RaycastHit2D hit = Physics2D.Raycast(position, direction, 1000, bulletLayerMask);

        ParticleSystem hitEffect = backgunHitEffects[gunIndex];

        if (!hit)
        {
            trail.AddPosition(position);
            trail.transform.position = position + direction * Random.Range(minigunTrailOffsetMin, minigunTrailOffsetMax) + direction * Random.Range(minigunTrailDistanceMin, minigunTrailDistanceMax);
        }
        else
        {
            float originDistance = Random.Range(minigunTrailHitOffsetMinRatio, minigunTrailHitOffsetMaxRatio) * hit.distance;
            float trailDistance = Random.Range(minigunTrailHitDistanceMinRatio, minigunTrailHitDistanceMaxRatio) * hit.distance;

            Vector2 origin = position + direction * originDistance;
            trail.AddPosition(origin);
            trail.transform.position = origin + direction * trailDistance;
            
            hitEffect.transform.position = hit.point;
            hitEffect.transform.rotation = Quaternion.LookRotation(direction, Vector3.forward);

            if (!hitEffect.isEmitting)
            {
                hitEffect.time = 0;
                hitEffect.Play();
            }

            lastBackgunHitEffect[gunIndex] = Time.time;

            backgunHitSE[gunIndex].Play();

            fightStatus = hit.collider.gameObject.GetComponent<FightStatus>();
            if (fightStatus)
            {
                fightStatus.Hurt(power);
            }
        }
    }

    public void Fire(Vector2 position, Vector2 direction, float speed, BulletType bulletType, int power)
    {
        if (bulletType == BulletType.handgun)
        {
            // Create bullet object
            GameObject obj = Instantiate(bulletPrefabs[(int)bulletType], position - direction * speed * Time.fixedDeltaTime, Quaternion.identity);

            obj.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), direction);

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            rb.velocity = direction * speed;

            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.damage = power;
            bullet.src = sourceType;
            bullet.spawnSource = gameObject;
            bullet.speed = speed;
            bullet.direction = direction;

            Destroy(obj, 15);
        }
        else if (bulletType == BulletType.shotgun)
        {
            // Create bullet object
            GameObject obj = Instantiate(bulletPrefabs[(int)bulletType], position - direction * speed * Time.fixedDeltaTime, Quaternion.identity);

            obj.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), direction);

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            rb.velocity = direction * speed;

            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.damage = power;
            bullet.tag = tag;
            bullet.spawnSource = gameObject;
            bullet.speed = speed;
            bullet.direction = direction;

            Destroy(obj, 15);
        }

        else if (bulletType == BulletType.maingun)
        {
            int bulletCount = Random.Range(mainGunBulletCountMin, mainGunBulletCountMax);
            for (int i = 0; i < bulletCount; i++)
            {

                // Create bullet object
                GameObject obj = Instantiate(bulletPrefabs[(int)bulletType], position - direction * speed * Time.fixedDeltaTime, Quaternion.identity);

                Vector3 targetAngle = Quaternion.LookRotation(new Vector3(0, 0, 1), direction).eulerAngles;
                targetAngle.z += Random.Range(mainGunSpreadAngleMin, mainGunSpreadAngleMax);
                obj.transform.rotation = Quaternion.Euler(targetAngle);

                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

                Bullet bullet = obj.GetComponent<Bullet>();
                bullet.damage = power;
                bullet.tag = tag;
                bullet.spawnSource = gameObject;
                bullet.speed = speed + Random.Range(mainGunBulletSpeedOffsetMin, mainGunBulletSpeedOffsetMax);
                bullet.direction = obj.transform.up;

                Destroy(obj, Random.Range(mainGunBulletDurationMin, mainGunBulletDurationMax));
            }
        }

    }
}
