using UnityEngine;
using Mirror;
using System.Collections.Generic;
using Mirror.Examples.MultipleAdditiveScenes;

public class SphereDegat : NetworkBehaviour
{
    [SerializeField] private List<GameObject> player = new List<GameObject>();
    [SerializeField] private NetworkAnimator na = null;
    [SerializeField] private float degatPlayer = 50f;
    [SerializeField] private float degatAfter = 1f;
    [SerializeField] private float destroyAfter = 2f;

    void Start()
    {
        if (!isServer) { return; }
        Invoke(nameof(DegatPlayer), degatAfter);
        Invoke(nameof(Destroy), destroyAfter);
        player = GameObject.Find("NetworkManager").GetComponent<GameNetworkManager>().GetPlayerList();
    }

    [Server]
    public void DegatPlayer()
    {
        foreach (GameObject p in player)
        {
            if (p)
            {
                p.GetComponent<Player>().ReceiveDamage(degatPlayer);
            }
        }
        na.SetTrigger("Active");
    }

    [Server]
    public void Destroy()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "nlPlayer" || other.transform.tag == "Player")
        {
            player.Remove(other.gameObject);
        }
    }

    [ServerCallback]
    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "nlPlayer" || other.transform.tag == "Player")
        {
            player.Add(other.gameObject);
        }
    }
}
