using UnityEngine;
using Mirror;
using System.Collections.Generic;

abstract public class ItemAbstract : NetworkBehaviour
{
    [SerializeField] protected SpawnWeapon sw = null;
    #region Start & Stop Callbacks
    [Server]
    public virtual void SetScript(SpawnWeapon s)
    {
        this.sw = s;
    }
    #endregion
}
