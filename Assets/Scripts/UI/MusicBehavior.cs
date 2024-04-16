using System;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class MusicBehavior : MonoBehaviour
{
    internal AudioSource m_audioSource;
    internal String m_musicPath = "Audio/";
    internal String[] m_musicFiles = new String[5] { "BGM0", "BGM1", "BGM2", "BGM3", "BGM4" };

    internal AudioClip[] m_musicClips =
        new AudioClip[5]; // https://forum.unity.com/threads/serialize-readonly-field.426525/

    private void OnSanChangedMusicFile(int value)
    {
        if (value <= 20)
        {
            m_audioSource.clip = m_musicClips[4];
        }
        else if (value <= 40)
        {
            m_audioSource.clip = m_musicClips[3];
        }
        else if (value <= 60)
        {
            m_audioSource.clip = m_musicClips[2];
        }
        else if (value <= 80)
        {
            m_audioSource.clip = m_musicClips[1];
        }
        else
        {
            m_audioSource.clip = m_musicClips[0];
        }
        m_audioSource.Play();
    }

    private void OnStageChangedMusicFile(int stage)
    {
        if (stage == (int)GameSession.SessionStage.Dialogue)
        {
            m_audioSource.clip = m_musicClips[2];
            m_audioSource.Play();
        }
        else if (stage == (int)GameSession.SessionStage.Summon)
        {
            m_audioSource.clip = m_musicClips[1];
            m_audioSource.Play();
        }
        else if (stage == (int)GameSession.SessionStage.GoodEnd)
        {
            // m_audioSource.clip = m_musicClips[4];
        }
        else
        {
            // m_audioSource.clip = m_musicClips[0];
        }
    }

    private void Start()
    {
        GameSession.Instance.m_valueSanChangedEvent += OnSanChangedMusicFile;
        GameSession.Instance.m_enumStageChangedEvent += OnStageChangedMusicFile;
        m_audioSource = GetComponent<AudioSource>();
        m_musicClips[0] = Resources.Load<AudioClip>(m_musicPath + m_musicFiles[0]);
        m_musicClips[1] = Resources.Load<AudioClip>(m_musicPath + m_musicFiles[1]);
        m_musicClips[2] = Resources.Load<AudioClip>(m_musicPath + m_musicFiles[2]);
        m_musicClips[3] = Resources.Load<AudioClip>(m_musicPath + m_musicFiles[3]);
        m_musicClips[4] = Resources.Load<AudioClip>(m_musicPath + m_musicFiles[4]);
        Debug.Log("Loaded " + m_musicFiles[0]);
    }
}
