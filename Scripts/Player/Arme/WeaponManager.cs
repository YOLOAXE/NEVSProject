using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using DitzelGames.FastIK;
using UnityEngine.UI;

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
    [SerializeField] private bool isUnlock = false;
    [SerializeField] private Sprite weaponImage = null;
    [SerializeField] private Sprite munImage = null;

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

    public void unlockWeapon()
    {
        this.isUnlock = true;
    }
    public bool IsUnlock()
    {
        return this.isUnlock;
    }

    public Sprite getImageArme()
    {
        return this.weaponImage;
    }

    public Sprite getMunImage()
    {
        return this.munImage;
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
    [SerializeField] private AudioClip clipGetArme = null;
    [SyncVar(hook = nameof(ChangeWeapon))] public int currentIDArme = 0;
    [Header("GunEffect")]
    [SerializeField] private impactEffect[] ie = null;
    [SerializeField] private CameraShake cShake = null;
    [Header("GunUI")]
    [SerializeField] private TextMeshProUGUI textMun = null;
    [SerializeField] private Image typeArme = null;
    [SerializeField] private GameObject addArmeImage = null;

    #region Server System Callbacks
    public override void OnStartLocalPlayer()
    {
        textMun = GameObject.Find("TMPMunition").GetComponent<TextMeshProUGUI>();
        typeArme = GameObject.Find("ImageArmeSelectioner").GetComponent<Image>();
        addArmeImage = GameObject.Find("ImageLootObject");
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
        if (GameInputManager.GetKey("Tire") || GameInputManager.GetKeyUp("Tire"))
        {
            StartCoroutine(wI[currentIDArme].getArmeScript().shoot());
        }
        if (GameInputManager.GetKey("Recharger"))
        {
            StartCoroutine(wI[currentIDArme].getArmeScript().reload());
        }
        wI[this.currentIDArme].getArmeScript().AimArme(GameInputManager.GetKey("Viser"));
        if (GameInputManager.GetKeyDown("ArmeSuivante"))
        {
            wI[this.currentIDArme].getArmeScript().OnChangeWeapon();
            CmdChangeWeapon(true);
        }
        if (GameInputManager.GetKeyDown("ArmePrecedente"))
        {
            wI[this.currentIDArme].getArmeScript().OnChangeWeapon();
            CmdChangeWeapon(false);
        }
        handAnimator.SetBool("Aim", GameInputManager.GetKey("Viser"));
    }

    [Command]
    private void CmdChangeWeapon(bool state)
    {
        int baseId = this.currentIDArme;
        int increment = (state ? 1 : -1);
        do
        {
            baseId = ((baseId + increment) % wI.Length);
            if(baseId < 0)
            {
                baseId = (wI.Length - 1);
            }
        }
        while (!wI[baseId].IsUnlock());    
        if (m_audioSource && clipChangeArme && this.currentIDArme != baseId)
        {
            m_audioSource.clip = clipChangeArme;
            m_audioSource.PlayOneShot(m_audioSource.clip);
        }
        this.currentIDArme = baseId;
        for (byte i = 0; i < wI.Length; i++)
        {
            wI[i].SpawnObject(i == this.currentIDArme);
        }
        handAnimator.Rebind();
        handAnimator.SetInteger("id", this.currentIDArme);
    }

    private void ChangeWeapon(int oldID, int newID)
    {
        for (byte i = 0; i < wI.Length; i++)
        {
            wI[i].SpawnObject(i == newID);
        }
        if (m_audioSource && clipChangeArme && oldID != newID)
        {
            m_audioSource.clip = clipChangeArme;
            m_audioSource.PlayOneShot(m_audioSource.clip);
        }
        this.currentIDArme = newID;
        handAnimator.Rebind();
        handAnimator.SetInteger("id", this.currentIDArme);
        wI[this.currentIDArme].getArmeScript().OnSelectWeapon();
        if (this.typeArme)
        {
            this.typeArme.sprite = wI[this.currentIDArme].getImageArme();
            this.typeArme.SetNativeSize();
        }
    }

    [Server]
    public bool UnlockWeapon(int id)
    {
        if (id >= 0 && id < wI.Length)
        {
            if (wI[id].IsUnlock()){return false;}
            wI[id].unlockWeapon();            
            RpcClipUnlockGetArme(id);
            return true;
        }
        return false;
    }

    [Server]
    public bool AddAmmo(int munCharg)
    {
        bool state = wI[this.currentIDArme].getArmeScript().AddMunition(munCharg);
        if(state)
        {
            RpcClipAmmo(wI[this.currentIDArme].getArmeScript().GetMultByAddMun(munCharg));
        }
        return state;
    }

    [ClientRpc]
    public void RpcClipAmmo(int ammo)
    {
        if (addArmeImage)
        {
            addArmeImage.GetComponent<Image>().sprite = wI[this.currentIDArme].getMunImage();
            addArmeImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + ammo.ToString();
            addArmeImage.GetComponent<Image>().SetNativeSize();
            addArmeImage.GetComponent<Animator>().SetBool("Add", true);
        }
        if (clipGetArme)
        {
            m_audioSource.clip = clipGetArme;
            m_audioSource.PlayOneShot(m_audioSource.clip);
        }
    }

    [ClientRpc]
    public void RpcClipUnlockGetArme(int id)
    {
        if (addArmeImage)
        {
            addArmeImage.GetComponent<Image>().sprite = wI[id].getImageArme();
            addArmeImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            addArmeImage.GetComponent<Image>().SetNativeSize();
            addArmeImage.GetComponent<Animator>().SetBool("Add", true);
        }
        if (clipGetArme)
        {
            m_audioSource.clip = clipGetArme;
            m_audioSource.PlayOneShot(m_audioSource.clip);
        }
    }
    public GameObject getImpactByTag(string tag)
    {
        foreach (impactEffect iE in ie)
        {
            if (iE.getTag() == tag)
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

    [ClientRpc]
    public void RpcSendMunition(int id, int mun, int charg)
    {
        wI[id].getArmeScript().OnChangeCM(mun, charg, id == this.currentIDArme);
    }

    public void SetTextMun(string txt)
    {
        if (textMun)
        {
            textMun.text = txt;
        }
    }

    public void StartcShake(float duration, float magnitude)
    {
        StartCoroutine(this.cShake.Shake(duration, magnitude));
    }

    public void SetDeath(bool state)
    {
        this.textMun.gameObject.SetActive(state);
        this.typeArme.gameObject.SetActive(state);
        this.addArmeImage.SetActive(state);
        wI[this.currentIDArme].getArmeScript().OnChangeWeapon();
        CmdChangeWeapon(state);
    }
}
