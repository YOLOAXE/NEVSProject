using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.MultipleAdditiveScenes;

public class PointDeSauvegarde : NetworkBehaviour
{
    [Header("Respawn Setting")]
    [SerializeField] private GameObject[] spawnPoint = null;
    [SerializeField] private int unLock = 0;
    [SerializeField] private List<GameObject> player = new List<GameObject>();
    [SerializeField] private float tpAfter = 3f;
    [Header("Audio Setting")]
    [SerializeField] private AudioSource m_audio = null;
    [SerializeField] private AudioClip respawn = null;
    [Header("Boss Respawn")]
    [SerializeField] private GameObject boss = null;
    [SerializeField] private GameObject bossObject = null;
    [SerializeField] private GameObject spawnPointBoss = null;


    #region Start & Stop Callbacks

    [Server]
    public void CheckAllPlayerForRespon()
    {
        this.player = GameObject.Find("NetworkManager").GetComponent<GameNetworkManager>().GetPlayerList();
        foreach(GameObject p in player)
        {
            if (p)
            {
                if (p.GetComponent<Player>().GetIsAlive())
                {
                    return;
                }
            }
        }
        Invoke(nameof(TpAllPlayer), this.tpAfter);
    }

    [Server]
    private void TpAllPlayer()
    {
        foreach (GameObject p in player)
        {
            if (p)
            {
                p.GetComponent<Player>().Receiveheal(100);
            }
        }
        this.m_audio.clip = this.respawn;
        this.m_audio.PlayOneShot(this.m_audio.clip);
        StartCoroutine(tpb());
        this.RpcTpAllPlayer(this.unLock);
    }

    IEnumerator tpb()
    {
        float timer = 1.0f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            foreach (GameObject p in this.player)
            {
                p.transform.position = spawnPoint[unLock].transform.position;
            }
            yield return null;
        }
        yield return null;
    }

    [ClientRpc]
    private void RpcTpAllPlayer(int indice)
    {
        GameObject pl = GameObject.FindWithTag("Player");
        if(pl)
        {
            pl.transform.position = spawnPoint[indice].transform.position;
            this.m_audio.clip = this.respawn;
            this.m_audio.PlayOneShot(this.m_audio.clip);
        }
    }

    [Server]
    public void ChangeSpawnPoint(GameObject obj)
    {
        for(int i = 0; i < this.spawnPoint.Length;i++)
        {
            if(obj == this.spawnPoint[i])
            {
                unLock = i;
                return;
            }
        }
    }

    #endregion
}
