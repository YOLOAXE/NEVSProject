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
        [SerializeField] private AArme a_scriptArme = null;

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

        public AArme getArmeScript()
        {
            return this.a_scriptArme;
        }
        public GameObject getArme()
        {
            return this.arme;
        }
    }

    public class WeaponManager : NetworkBehaviour
    {
        [SerializeField] private Hand[] tHand = null;
        [SerializeField] private WeaponInfo[] wI = null;
        [Header("WeaponSetting")]
        [SerializeField] private GameObject pivotCam = null;
        [SerializeField] private GameObject handObject = null;
        [SerializeField] private Animator handAnimator = null;
        [SerializeField] private AudioSource m_audioSource = null;
        [SerializeField] private AudioClip clipChangeArme = null;
        [SyncVar] public int currentIDArme = 0;

        public override void OnStartLocalPlayer()
        {
            /*foreach (WeaponInfo w in wI)
            {
                if (w.getArme())
                {

                }
            }*/
            Debug.Log(wI[2].getArme().GetComponent<NetworkIdentity>());
            //CmdAssignAuthority(wI[2].getArme().GetComponent<NetworkIdentity>());
        }

        void LateUpdate()
        {
            for (byte i = 0; i < tHand.Length; i++)
            {
                tHand[i].AplyAnimation(wI[currentIDArme].GetFreeHand(i));
            }
        }

        void Update()
        {
            if (!isLocalPlayer) { return; }
            if (Input.GetButton("Fire1"))
            {
                StartCoroutine(wI[currentIDArme].getArmeScript().shoot());
            }
            if (Input.GetButton("Reload"))
            {
                StartCoroutine(wI[currentIDArme].getArmeScript().reload());
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                CmdChangeWeapon(++currentIDArme % wI.Length);
            }
            handAnimator.SetBool("Aim", Input.GetButton("Fire2"));
            handObject.transform.eulerAngles = pivotCam.transform.eulerAngles;
        }

        [Command]
        private void CmdChangeWeapon(int v)
        {
            for (byte i = 0; i < wI.Length; i++)
            {
                wI[i].SpawnObject(i == v);
            }
            if (m_audioSource && clipChangeArme)
            {
                m_audioSource.clip = clipChangeArme;
                m_audioSource.PlayOneShot(m_audioSource.clip);
            }
            handAnimator.SetBool("return", true);
            handAnimator.SetInteger("id", v);
            currentIDArme = v;
        }

        [Command]
        void CmdTire()
        {

        }

    }
}