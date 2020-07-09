using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class ObjectRb : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    #region Start & Stop Callbacks

    public override void OnStartClient()
    {
        if (isServer) { return; }
        rb.isKinematic = true;
    }
    
    #endregion
}
