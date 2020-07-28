using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using VHS;

public class Boss : NetworkBehaviour
{
    [Header("Boss Setting")]
    [SerializeField] private float currentHealth = 100.0f;
    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private bool dead = false;
    [SerializeField] private bool inFight = false;
    [Header("Deplacement")]
    [SerializeField] private GameObject tpPlayerPoint = null;
    [SerializeField] private List<GameObject> AllPlayer = null;

    #region Start & Stop Callbacks
    [Server]
    public virtual void ReceiveAllPlayer(List<GameObject> al)
    {
        this.AllPlayer.Clear();
        this.AllPlayer = al;
    }

    public void StartFight()
    {
        this.RpcTpAllPlayer();
        this.inFight = true;
        StartCoroutine(tpb());
    }
    #endregion

    IEnumerator tpb()
    {
        float timer = 1.0f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            foreach (GameObject p in this.AllPlayer)
            {
                p.transform.position = tpPlayerPoint.transform.position;
            }
            yield return null;
        }
    }

    [ClientRpc]
    private void RpcTpAllPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            player.transform.position = tpPlayerPoint.transform.position;
        }
    }
}
