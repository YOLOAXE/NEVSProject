using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace DitzelGames.FastIK
{
    abstract public class AArme : NetworkBehaviour
    {
        [SerializeField] protected WeaponManager wM = null;
        [SerializeField] protected NetworkAnimator netAnim = null;
        public virtual IEnumerator shoot() { yield return null; }
        public virtual IEnumerator reload() { yield return null; }
        public virtual void CmdSendTire() {}
        public virtual void AimArme(bool state) {}
        public virtual IEnumerator CmdSendReload() { yield return null; }
    }
}
