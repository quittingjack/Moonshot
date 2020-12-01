using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUp : MonoBehaviour
{
    public Vector3 initScale;
    public float speed;

    public void StartScaleUp()
    {
        transform.localScale = initScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = transform.localScale;
        scale += Vector3.one * speed * Time.deltaTime;
        transform.localScale = scale;
    }
}
