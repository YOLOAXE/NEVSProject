using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class RPoint : NetworkBehaviour
{
    [Header("RespawnPoint")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private PointDeSauvegarde pds = null;
    [SerializeField] private GameObject[] objectToDesactive = null;
    [Header("Audio Setting")]
    [SerializeField] private AudioSource m_audio = null;
    [SerializeField] private AudioClip obtien = null;

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player" || other.transform.tag == "nlPlayer")
        {
            if(pds && !isActive)
            {
                pds.ChangeSpawnPoint(gameObject);
                RpcODS();
                foreach (GameObject o in this.objectToDesactive)
                {
                    o.SetActive(false);
                }
                this.m_audio.clip = this.obtien;
                this.m_audio.PlayOneShot(this.m_audio.clip);
                isActive = true;
            }
        }
    }

    [ClientRpc]
    private void RpcODS()
    {
        foreach(GameObject o in this.objectToDesactive)
        {
            o.SetActive(false);
        }
        this.m_audio.clip = this.obtien;
        this.m_audio.PlayOneShot(this.m_audio.clip);
    }
}
