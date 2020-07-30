using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class BossZoneManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] ObjectActiveInFight = null;
    [SerializeField] private bool bossFight = false;
    [SerializeField] private bool endFight = false;
    [SerializeField] private Boss b = null;

    #region Start & Stop Callbacks

    public override void OnStartServer() { }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "nlPlayer" || other.transform.tag == "Player")
        {
            if (!endFight && !bossFight)
            {
                bossFight = true;
                RpcactiveZone(this.bossFight);
                b.StartFight();
            }
        }
    }

    [Server]
    public void AplyEndFight()
    {
        this.endFight = true;
        this.bossFight = false;
        RpcactiveZone(this.bossFight);
    }

    [ClientRpc]
    private void RpcactiveZone(bool state)
    {
        foreach(GameObject o in ObjectActiveInFight)
        {
            o.SetActive(state);
        }
    }

    [Server]
    public void SetendFight(bool state)
    {
        this.endFight = state;
    }

    [Server]
    public void ResetBoss()
    {
        if (!endFight)
        {
            bossFight = false;
            RpcactiveZone(false);
            b.rBoss();
        }
    }
    #endregion
}
