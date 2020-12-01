using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOutAudioSource : MonoBehaviour
{
    public AudioSource audioSource;
    public bool fading;

    public float targetVolume;
    public float fadeSpeed;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
            targetVolume = audioSource.volume;
    }

    public void FadeTo(float duration, float targetVolume)
    {
        float diff = targetVolume - audioSource.volume;
        fadeSpeed = diff / duration;
        this.targetVolume = targetVolume;
        fading = true;
    }

    private void Update()
    {
        float step = fadeSpeed * Time.deltaTime;
        if (Mathf.Abs(audioSource.volume - targetVolume) < Mathf.Abs(step))
        {
            audioSource.volume = targetVolume;
            fading = false;
        }
        else
        {
            audioSource.volume += step;
        }
    }
}
