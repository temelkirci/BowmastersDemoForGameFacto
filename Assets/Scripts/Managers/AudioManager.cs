using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get { return instance; }
    }

    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip ThrowSound;
    public AudioClip HitSound;
    public AudioClip WinSound;
    public AudioClip LoseSound;

    void Start()
    {
        instance = this;
    }

    public void PlayHitSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(HitSound);
    }
    public void PlayThrowSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(ThrowSound);
    }
    public void PlayWinSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(WinSound);
    }
    public void PlayLoseSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(LoseSound);
    }
}
