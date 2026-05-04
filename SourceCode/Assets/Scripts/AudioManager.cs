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

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ApplyVolume();
        PlayMusic();
    }

    public void PlaySFX(string sound)
    {
        AudioClip clip = sound switch
        {
            "walking" => walking,
            "jump" => jump,
            "rope" => rope,
            "metal" => metal,
            "wee" => wee,
            _ => null
        };

        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    void PlayMusic()
    {
        if (backgroundMusic == null) return;

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void ApplyVolume()
    {
        sfxSource.volume = sfxVolume;
        musicSource.volume = musicVolume;
    }
}



