using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhiteScreen : MonoBehaviour
{
    public float fadeInTime;
    public float fadeOutTime;
    public float stayTime;

    [SerializeField] Image img;
    float startTime;

    private void Start()
    {
        startTime = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        Color c = Color.white;
        float timePassed = Time.time - startTime;
        if (timePassed < fadeInTime)
        {
            c.a = timePassed / fadeInTime;
        }
        else if (timePassed >= fadeInTime && timePassed < (fadeInTime + stayTime))
        {
            c.a = 1;
        }
        else
        {
            c.a = 1 - ((Time.time - startTime - fadeInTime - stayTime) / fadeOutTime);
        }
        img.color = c;
    }
}
