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

        public int getId()
        {
            return this.idArme;
        }
    }

    public class WeaponManager : NetworkBehaviour
    {
        [SerializeField] private Hand[] tHand = null;
        [SerializeField] private WeaponInfo[] wI = null;
        [SerializeField] private GameObject pivotCam = null;
        [SerializeField] private GameObject handObject = null;
        [SerializeField] private Animator handAnimator = null;
        [SerializeField] private AudioSource m_audioSource = null;
        [SerializeField] private AudioClip clipChangeArme = null;
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
                CmdChangeWeapon(++currentIDArme%wI.Length);
            }
            handObject.transform.eulerAngles = pivotCam.transform.eulerAngles;
        }

        [Command]
        public void CmdChangeWeapon(int v)
        {
            for(byte i = 0; i < wI.Length;i++)
            {
                wI[i].SpawnObject(i != v);
            }
            if (m_audioSource && clipChangeArme)
            {
                m_audioSource.clip = clipChangeArme;
                m_audioSource.PlayOneShot(m_audioSource.clip);
            }
            handAnimator.SetInteger("id", v);
            handAnimator.Play("idelHand", 0, 0.25f);
            currentIDArme = v;
        }
    }
}