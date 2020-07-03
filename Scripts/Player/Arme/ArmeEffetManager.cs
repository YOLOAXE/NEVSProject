using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmeEffect
{
    [SerializeField] private string name = ""; 
    [SerializeField] private AudioClip m_Shoot = null;
    [SerializeField] private AudioClip m_reload = null;
    [SerializeField] private AudioClip m_noAmmo = null;
    [SerializeField] private AudioClip m_endReload = null;
    [SerializeField] private ParticleSystem ps = null;

    public AudioClip getShootSound()
    {
        return this.m_Shoot;
    }

    public AudioClip getReloadSound()
    {
        return this.m_reload;
    }

    public AudioClip getEndReload()
    {
        return this.m_endReload;
    }

    public AudioClip getNoAmmoSound()
    {
        return this.m_noAmmo;
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
        m_audioSource.clip = AE[id].getReloadSound();
        m_audioSource.PlayOneShot(m_audioSource.clip);
    }

    public void endReloadSound(int id)
    {
        m_audioSource.clip = AE[id].getEndReload();
        m_audioSource.PlayOneShot(m_audioSource.clip);
    }

    public void noAmmoSound(int id)
    {
        m_audioSource.clip = AE[id].getNoAmmoSound();
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
