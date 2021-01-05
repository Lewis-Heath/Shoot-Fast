using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    [SerializeField] public string m_Name;
    [SerializeField] public AudioClip m_Clip;

    [Range(0f,1f)]
    [SerializeField] public float m_Volume;

    [Range(0.1f, 3f)]
    [SerializeField] public float m_Pitch;
    [Range(0.1f, 3f)]
    [SerializeField] public float m_MaxPitch;
    [Range(0.1f, 3f)]
    [SerializeField] public float m_MinPitch;

    [SerializeField] public bool m_looping;

    [HideInInspector]
    public AudioSource m_Source;
}
