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
            StartCoroutine(AddPlayerDelayed(conn));
        }
        public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
        {
            allPlayer.Remove(conn.identity.transform.gameObject);
        }

        IEnumerator AddPlayerDelayed(NetworkConnection conn)
        {
            yield return new WaitForSeconds(0.1f);
            base.OnServerAddPlayer(conn);
            allPlayer.Add(conn.identity.transform.gameObject);
        }
        #endregion
    }
}