using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class ImpactGlue : NetworkBehaviour
{
    [SerializeField] private float destroyAfter = 5;
    [SerializeField] private AudioSource m_audio = null;
    [SerializeField] private AudioClip[] m_clip = null;

    void Start()
    {
        if(m_audio)
        {
            m_audio.clip = m_clip[Random.Range(0,m_clip.Length)];
            m_audio.PlayOneShot(m_audio.clip);
        }
    }

    #region Start & Stop Callbacks
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion
}
