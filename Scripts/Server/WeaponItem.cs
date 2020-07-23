using UnityEngine;
using Mirror;
using System.Collections.Generic;
using DitzelGames.FastIK;

public class WeaponItem : ItemAbstract
{
    [SerializeField] private int idWeapon = 0;
    private bool takeByPlayer = false;

    #region Start & Stop Callbacks

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    void OnTriggerStay(Collider other)
    {
        if (!takeByPlayer && (other.transform.tag == "nlPlayer" || other.transform.tag == "Player"))
        {            
            if (other.transform.GetComponent<WeaponManager>().UnlockWeapon(this.idWeapon))
            {
                takeByPlayer = true;
                if(base.sw)
                {
                    base.sw.getObjectByPlayer(gameObject);
                }
                DestroySelf();
            }
        }
    }

    #endregion
}
