using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoundary : MonoBehaviour
{
    Transform u;
    Transform d;
    Transform l;
    Transform r;

    private void Awake()
    {
        u = transform.Find("U");
        d = transform.Find("D");
        r = transform.Find("R");
        l = transform.Find("L");
        u.gameObject.SetActive(false);
        d.gameObject.SetActive(false);
        r.gameObject.SetActive(false);
        l.gameObject.SetActive(false);
    }

    public Vector3 GetRandomPosInBound()
    {
        Vector3 ret = Vector3.zero;
        ret.x = Random.Range(l.position.x, r.position.x);
        ret.y = Random.Range(d.position.y, u.position.y);
        return ret;
    }
}
