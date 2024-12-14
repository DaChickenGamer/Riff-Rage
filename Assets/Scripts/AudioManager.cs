using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public AudioClip[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
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
    
    public void PlayMusic(string name)
    {
        AudioClip sound = Array.Find(musicSounds, song => song.name == name);
        
        if (sound == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = sound;
            musicSource.Play();
        }
    }
    
    public void PlaySFX(string name)
    {
        AudioClip sound = Array.Find(sfxSounds, sfx => sfx.name == name);
        
        if (sound == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(sound);
        }
    }
}
