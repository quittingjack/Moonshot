using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletSource
{
    player,
    enemy
}

public class Bullet : MonoBehaviour
{
    public GameObject effectPrefab;
    public int damage;
    public BulletSource src;
    protected SpriteRenderer[] srs;
    protected Rigidbody2D rb;
    public AudioSource hitSE;
    public GameObject spawnSource;
    public Vector2 direction;
    public float speed;

    public float increaseVelOverTime;

    private void Awake()
    {
        srs = GetComponentsInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        hitSE = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        Vector2 vel = direction * speed;
        rb.velocity = vel;
        speed += increaseVelOverTime * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag == "Player" && src == BulletSource.enemy) ||
            (collision.tag == "Enemy" && src == BulletSource.player))
        {
            if (effectPrefab != null)
            {
                Instantiate(effectPrefab, transform.position, Quaternion.LookRotation(rb.velocity.normalized, Vector3.forward));
            }
            FightStatus fightStatus = collision.gameObject.GetComponent<FightStatus>();
            if (fightStatus)
            {
                fightStatus.Hurt(damage);
            }
            foreach (SpriteRenderer sr in srs)
            {
                sr.enabled = false;
            }
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(gameObject, 1);

            if (hitSE)
            {
                hitSE.Play();
            }
        }
    }
}
