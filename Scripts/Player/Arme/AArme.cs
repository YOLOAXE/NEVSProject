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
        [SerializeField] protected int idArme = 0;
        public virtual IEnumerator shoot() { yield return null; }
        public virtual IEnumerator reload() { yield return null; }
        public virtual void CmdSendTire() {}
        public virtual bool AddMunition(int munCharg) { return false; }
        public virtual int GetMultByAddMun(int munCharg) { return 0; }
        public virtual void AimArme(bool state) { }
        public virtual void OnChangeWeapon() { }
        public virtual void OnSelectWeapon() { }
        public virtual void OnChangeCM(int mun,int charg,bool draw) { }
        public virtual IEnumerator CmdSendReload() { yield return null; }
    }
}
