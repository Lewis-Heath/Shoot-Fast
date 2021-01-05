using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] m_Sounds;
    private GameManager m_GameManager;

    void Awake()
    {
        foreach (Sound sound in m_Sounds)
        {
            sound.m_Source = gameObject.AddComponent<AudioSource>();
            sound.m_Source.clip = sound.m_Clip;
            sound.m_Source.volume = sound.m_Volume;
            sound.m_Source.pitch = sound.m_Pitch;
            sound.m_Source.loop = sound.m_looping;
        }
    }

    private void Start()
    {
        m_GameManager = FindObjectOfType<GameManager>();
    }

    public void PlaySound(string name)
    {
        if(m_GameManager.m_CurrentScreen == ScreenType.Playing)
        {
            Sound soundToPlay = Array.Find(m_Sounds, sound => sound.m_Name == name);
            soundToPlay.m_Source.Play();
        }
    }

    public void StopSound(string name)
    {
        Sound soundToStop = Array.Find(m_Sounds, sound => sound.m_Name == name);
        soundToStop.m_Source.Stop();
    }

    public bool IsPlayingSound(string name)
    {
        Sound soundToCheck = Array.Find(m_Sounds, sound => sound.m_Name == name);
        return soundToCheck.m_Source.isPlaying;
    }

    public void PauseSound(string name)
    {
        Sound soundToPause = Array.Find(m_Sounds, sound => sound.m_Name == name);
        soundToPause.m_Source.Pause();
    }

    public void UnPauseSound(string name)
    {
        Sound soundToUnpause = Array.Find(m_Sounds, sound => sound.m_Name == name);
        soundToUnpause.m_Source.Pause();
    }

    public void RandomizePitchSound(string name)
    {
        Sound soundToChange = Array.Find(m_Sounds, sound => sound.m_Name == name);
        float pitch = UnityEngine.Random.Range(soundToChange.m_MinPitch, soundToChange.m_MaxPitch);
        soundToChange.m_Source.pitch = pitch;
    }
}
