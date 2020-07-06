using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

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

    [System.Serializable]
    public class impactEffect
    {
        [SerializeField] private string tagName = "";
        [SerializeField] private GameObject spawnImpact = null;

        public string getTag()
        {
            return this.tagName;
        }

        public GameObject getImpact()
        {
            return this.spawnImpact;
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
        [SyncVar(hook = nameof(ChangeWeapon))] public int currentIDArme = 0;
        [Header("GunEffect")]
        [SerializeField] private impactEffect[] ie = null;
        [SerializeField] private CameraShake cShake = null;
        [Header("GunUI")]
        [SerializeField] private TextMeshProUGUI textMun = null;

        #region Server System Callbacks
        public override void OnStartLocalPlayer()
        {
            textMun = GameObject.Find("TMPMunition").GetComponent<TextMeshProUGUI>();
        }
        #endregion

        void LateUpdate()
        {
            for (byte i = 0; i < tHand.Length; i++)
            {
                tHand[i].AplyAnimation(wI[currentIDArme].GetFreeHand(i));
            }
        }

        void Update()
        {
            handObject.transform.eulerAngles = pivotCam.transform.eulerAngles;
            if (!isLocalPlayer) { return; }
            if (Input.GetButton("Fire1"))
            {
                StartCoroutine(wI[currentIDArme].getArmeScript().shoot());
            }
            if (Input.GetButton("Reload"))
            {
                StartCoroutine(wI[currentIDArme].getArmeScript().reload());
            }
            wI[this.currentIDArme].getArmeScript().AimArme(Input.GetButton("Fire2"));
            if (Input.GetKeyDown(KeyCode.F))
            {
                wI[this.currentIDArme].getArmeScript().OnChangeWeapon();
                CmdChangeWeapon();
            }
            handAnimator.SetBool("Aim", Input.GetButton("Fire2"));
        }

        [Command]
        private void CmdChangeWeapon()
        {
            this.currentIDArme = ((this.currentIDArme + 1)%wI.Length);
            for (byte i = 0; i < wI.Length; i++)
            {
                wI[i].SpawnObject(i == this.currentIDArme);
            }
            if (m_audioSource && clipChangeArme)
            {
                m_audioSource.clip = clipChangeArme;
                m_audioSource.PlayOneShot(m_audioSource.clip);
            }
            handAnimator.Rebind();
            handAnimator.SetInteger("id", this.currentIDArme);
        }

        private void ChangeWeapon(int oldID,int newID)
        {
            for (byte i = 0; i < wI.Length; i++)
            {
                wI[i].SpawnObject(i == newID);
            }
            if (m_audioSource && clipChangeArme)
            {
                m_audioSource.clip = clipChangeArme;
                m_audioSource.PlayOneShot(m_audioSource.clip);
            }
            this.currentIDArme = newID;
            handAnimator.Rebind();
            handAnimator.SetInteger("id", this.currentIDArme);
            wI[this.currentIDArme].getArmeScript().OnSelectWeapon();
        }

        public GameObject getImpactByTag(string tag)
        {
            foreach(impactEffect iE in ie)
            {
                if(iE.getTag() == tag)
                {
                    return iE.getImpact();
                }
            }
            return ie[0].getImpact();
        }

        [Command]
        public void CmdTire()
        {
            wI[currentIDArme].getArmeScript().CmdSendTire();
        }

        [Command]
        public void CmdReload()
        {
            StartCoroutine(wI[currentIDArme].getArmeScript().CmdSendReload());
        }

        public void SetTextMun(string txt)
        {
            if (textMun)
            {
                textMun.text = txt;
            }
        }

        public void StartcShake(float duration,float magnitude)
        {
            StartCoroutine(this.cShake.Shake(duration, magnitude));
        }
    }
}