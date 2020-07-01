using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class DebugInfoManager : NetworkBehaviour
{
    #region Start & Stop Callbacks

    public override void OnStartServer() { }
    public override void OnStopServer() { }
    public override void OnStartClient() { }
    public override void OnStopClient() { }
    public override void OnStartLocalPlayer() { }
    public override void OnStartAuthority() { }
    public override void OnStopAuthority() { }

    #endregion
    [ClientRpc]
    public void RpcSendDebugToClient(string deb)
    {
        Debug.Log("Debug Consol Test Manager: " + deb);
    }
}
