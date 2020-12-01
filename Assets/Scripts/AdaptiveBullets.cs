using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveBullets : MonoBehaviour
{
    public Transform[] sprites;
    public Vector3[] startSize;
    public float[] sizeIncreaseRatio;
    public float[] holdTime;
    
    Vector3[] targetSize;
    float[] currentSize;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        targetSize = new Vector3[sprites.Length];
        currentSize = new float[sprites.Length];
        for (int i = 0; i < sprites.Length; i++) 
        {
            targetSize[i] = sprites[i].localScale;
            sprites[i].localScale = startSize[i];
            currentSize[i] = 0;
        }
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < sprites.Length; i++)
        {
            if ((Time.time - startTime) < holdTime[i])
                continue;
            Vector3 realSize = Vector3.Lerp(startSize[i], targetSize[i], currentSize[i]);
            sprites[i].localScale = realSize;
            currentSize[i] += sizeIncreaseRatio[i] * Time.deltaTime;
            if (currentSize[i] > 1)
                currentSize[i] = 1;
        }
    }
}
