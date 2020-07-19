using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmeEffect
{
    [SerializeField] private string name = ""; 
    [SerializeField] private AudioClip m_Shoot = null;
    [SerializeField] [Range(0.0f, 1.0f)] private float volume_m_Shoot = 0.1f;
    [SerializeField] private AudioClip m_reload = null;
    [SerializeField] [Range(0.0f, 1.0f)] private float volume_m_reload = 0.1f;
    [SerializeField] private AudioClip m_noAmmo = null;
    [SerializeField] [Range(0.0f, 1.0f)] private float volume_m_noAmmo = 0.1f;
    [SerializeField] private AudioClip m_endReload = null;
    [SerializeField] [Range(0.0f, 1.0f)] private float volume_m_endReload = 0.1f;
    [SerializeField] private ParticleSystem ps = null;
    [SerializeField] private ParticleSystem psAmmo = null;

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

    public ParticleSystem getPSAmmo()
    {
        return this.psAmmo;
    }

    public float getVolume_ShootSound()
    {
        return this.volume_m_Shoot;
    }

    public float getVolume_ReloadSound()
    {
        return this.volume_m_reload;
    }

    public float getVolume_EndReload()
    {
        return this.volume_m_endReload;
    }

    public float getVolume_NoAmmoSound()
    {
        return this.volume_m_noAmmo;
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
        m_audioSource.volume = AE[id].getVolume_ShootSound();
        m_audioSource.PlayOneShot(m_audioSource.clip);
    }

    public void reloadSound(int id)
    {
        m_audioSource.clip = AE[id].getReloadSound();
        m_audioSource.volume = AE[id].getVolume_ReloadSound();
        m_audioSource.PlayOneShot(m_audioSource.clip);
    }

    public void endReloadSound(int id)
    {
        m_audioSource.clip = AE[id].getEndReload();
        m_audioSource.volume = AE[id].getVolume_EndReload();
        m_audioSource.PlayOneShot(m_audioSource.clip);
    }

    public void noAmmoSound(int id)
    {
        m_audioSource.clip = AE[id].getNoAmmoSound();
        m_audioSource.volume = AE[id].getVolume_NoAmmoSound();
        m_audioSource.PlayOneShot(m_audioSource.clip);
    }

    public void PlayPS(int id)
    {
        AE[id].getPS().Play();
    }

    public void PlayPSAmmo(int id)
    {
        AE[id].getPSAmmo().Play();
    }

    public void StopPS(int id)
    {
        AE[id].getPS().Stop();
    }
}
