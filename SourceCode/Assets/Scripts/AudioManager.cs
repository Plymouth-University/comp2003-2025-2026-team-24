using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("SFX Clips")]
    public AudioClip walking;
    public AudioClip jump;
    public AudioClip rope;
    public AudioClip metal;
    public AudioClip wee;

    [Header("Music")]
    public AudioClip backgroundMusic;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        PlayMusic();
    }

    public void PlaySFX(string sound)
    {
        switch (sound)
        {
            case "walking":
                sfxSource.PlayOneShot(walking);
                break;

            case "jump":
                sfxSource.PlayOneShot(jump);
                break;

            case "rope":
                sfxSource.PlayOneShot(rope);
                break;

            case "metal":
                sfxSource.PlayOneShot(metal);
                break;

            case "wee":
                sfxSource.PlayOneShot(wee);
                break;
        }
    }

    void PlayMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
}