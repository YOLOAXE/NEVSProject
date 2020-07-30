using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using VHS;

namespace Mirror.Examples.MultipleAdditiveScenes
{
    public class GameNetworkManager : NetworkManager
    {
        [Header("Other")]
        [SerializeField] private List<GameObject> allPlayer = new List<GameObject>();

        #region Server System Callbacks
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            this.allPlayer.Add(conn.identity.transform.gameObject);
        }
        public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
        {
            allPlayer.Remove(conn.identity.transform.gameObject);
        }

        public List<GameObject> GetPlayerList()
        {
            return this.allPlayer;
        }
        #endregion
    }
}