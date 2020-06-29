using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmeEffect
{
    [SerializeField] private string name = ""; 
    [SerializeField] private AudioClip m_Shoot = null;
    [SerializeField] private AudioClip m_reload = null;
    [SerializeField] private ParticleSystem ps = null;

    public AudioClip getShootSound()
    {
        return this.m_Shoot;
    }

    public AudioClip getReloadSound()
    {
        return this.m_reload;
    }

    public ParticleSystem getPS()
    {
        return this.ps;
    }
}

public class ArmeEffetManager : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] private AudioSource m_audioSource = null;
    [SerializeField] private ArmeEffect[] AE = null;

    public void shootSound(int id)
    {
        m_audioSource.clip = AE[id].getShootSound();
        m_audioSource.PlayOneShot(m_audioSource.clip);
    }

    public void reloadSound(int id)
    {
        m_audioSource.clip = AE[id].getShootSound();
        m_audioSource.PlayOneShot(m_audioSource.clip);
    }

    public void PlayPS(int id)
    {
        AE[id].getPS().Play();
    }

    public void StopPS(int id)
    {
        AE[id].getPS().Stop();
    }
}
