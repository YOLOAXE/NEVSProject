using UnityEngine;
using Mirror;
using System.Collections.Generic;

abstract public class AttaqueBoss : NetworkBehaviour
{
    [SerializeField] protected Boss b = null;
    #region Start & Stop Callbacks
    public virtual void attaqueStart() {}
    public virtual void attaqueUpdate(){}
    public virtual void deplacement() {}
    public virtual void resetAt() { }
    #endregion
}
