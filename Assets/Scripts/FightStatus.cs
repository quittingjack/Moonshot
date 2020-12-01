using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightStatus : MonoBehaviour
{
    public GameObject hittedPrefab;
    public AudioSource hittedSE;
    public bool isInvincible;

    public int maxHP;
    public int power;

    [SerializeField] int HP;

    public int getHP()
    {
        return HP;
    }
    public void setHP(int val)
    {
        HP = val;
    }

    private void Awake()
    {
        HP = maxHP;
    }

    public int Hurt(int damage)
    {
        if (isInvincible) return 0;


        if (HP >= damage)
            HP -= damage;
        else
            HP = 0;

        if (HP <= 0)
            Death();

        if (damage != 0 && tag == "Player")
        {
            GameObject obj = Instantiate(hittedPrefab);
            obj.transform.position = transform.position;
            hittedSE.Play();
        }

        return damage;
    }

    void Death()
    {
        if (tag == "Player")
        {
            GetComponent<ForceControl>().Death();
        }
        // Debug.Log("Dead");
    }
}
