using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepEnemie : MonoBehaviour
{
    [SerializeField] private AudioSource m_audioSource = null;
    [Range(0f, 1f)] [SerializeField] private float volumeSound = 1.0f;
    [SerializeField] private AudioClip[] clipFoot = null;

    public void footStepSR()
    {
        m_audioSource.volume = volumeSound;
        m_audioSource.clip = clipFoot[Random.Range(0,clipFoot.Length)];
        m_audioSource.PlayOneShot(m_audioSource.clip);
    }
}
