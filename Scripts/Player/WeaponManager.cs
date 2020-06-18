using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace DitzelGames.FastIK
{
    [System.Serializable]
    public class Hand
    {
        [SerializeField] private GameObject rigHand = null;

        public GameObject getRH()
        {
            return this.rigHand;
        }
        public void AplyAnimation(bool state)
        {
            this.rigHand.GetComponent<FastIKFabric>().enabled = !state;
        }
    }
    [System.Serializable]
    public class WeaponInfo
    {
        [SerializeField] private string Name = "";
        [SerializeField] private int idArme = 0;
        [SerializeField] private bool[] freeHand = null;
        [SerializeField] private GameObject arme = null;

        public bool GetFreeHand(byte indice)
        {
            return this.freeHand[indice];
        }

        public void SpawnObject(bool state)
        {
            if (arme)
            {
                arme.SetActive(state);
            }
        }
    }

    public class WeaponManager : NetworkBehaviour
    {
        [SerializeField] private Hand[] tHand = null;
        [SerializeField] private WeaponInfo[] wI = null;
        [SerializeField] private GameObject pivotCam = null;
        [SerializeField] private GameObject handObject = null;
        [SyncVar] public int currentIDArme = 0;


        void LateUpdate()
        {
            for (byte i = 0; i < tHand.Length; i++)
            {
                tHand[i].AplyAnimation(wI[currentIDArme].GetFreeHand(i));
            }
        }
        void Update()
        {
            if(!isLocalPlayer)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                CmdChangeWeapon(currentIDArme == 0 ? 1 : 0);
            }
            handObject.transform.eulerAngles = pivotCam.transform.eulerAngles;
        }

        [Command]
        public void CmdChangeWeapon(int v)
        {
            for(byte i = 0; i < wI.Length;i++)
            {
                wI[i].SpawnObject(i == v);
            }
            currentIDArme = v;
        }
    }
}