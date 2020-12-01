using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightVelBullet : Bullet
{
    public float rightVelRatio;

    private void FixedUpdate()
    {


        Vector3 toSrc = (spawnSource.transform.position - transform.position).normalized;
        Vector3 rotated = Quaternion.AngleAxis(90.0f, Vector3.forward) * toSrc;
        // Debug.DrawLine(transform.position, rotated);
        Vector2 vel = -new Vector2(toSrc.x, toSrc.y) * speed + rightVelRatio * new Vector2(rotated.x, rotated.y);
        rb.velocity = vel;
        speed += increaseVelOverTime * Time.fixedDeltaTime;
    }
    
}
