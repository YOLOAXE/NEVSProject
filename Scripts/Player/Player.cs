using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using VHS;
using TMPro;

public class Player : NetworkBehaviour
{
    [Header("Name setting")]
    [SerializeField] [SyncVar(hook = "OnChangeName")] private string namePL = "";
    [SerializeField] private TextMeshProUGUI textName = null;
    [SerializeField] private Animator anim = null;
    [SerializeField] private CharacterController controller = null;
    [Header("Health Setting")]
    [SerializeField] [SyncVar(hook = "OnChangeHealth")] private float currenthealth = 100.0f;
    [SerializeField] private float maxhealth = 100.0f;
    [SerializeField] private float degatTime = 1f;
    [SerializeField] private Image sOHit = null;
    [SerializeField] private Image sO = null;
    private bool isHit = false;
    [Header("Health Regeneration")]
    [SerializeField] private float regenneRate = 5f;
    [SerializeField] private float regenneHealth = 10f;
    [SerializeField] [SyncVar(hook = "OnChangeAlive")] private bool isAlive = true;
    [Header("Dead setting")]
    [SerializeField] private GameObject[] objectToDisabled = null;
    [SerializeField] private GameObject[] objectToEnabled = null;
    [SerializeField] private Behaviour[] scriptToDisabled = null;
    [SerializeField] private GameObject[] head = null;
    [SerializeField] private GameObject torse = null;
    [Header("Audio")]
    [SerializeField] private AudioSource mouseAudioS = null;
    [SerializeField] private AudioSource heartAudioS = null;
    [SerializeField] private AudioClip[] hurtDegat = null;

    void Start()
    {
        if (!isLocalPlayer) { return; }
        textName.gameObject.SetActive(false);
        namePL = PlayerPrefs.GetString("Pseudo", "Joueur");
        textName.text = this.namePL;
        sO = GameObject.Find("SangOverlay").GetComponent<Image>();
        sOHit = GameObject.Find("SangOverlay2").GetComponent<Image>();
    }

    void Update()
    {
        if (!isLocalPlayer && isAlive)
        {
            return;
        }
        AplyAnimation();
    }

    #region Start & Stop Callbacks

    public override void OnStartServer()
    {
        StartCoroutine(RegenHealth());
    }

    #endregion

    private void AplyAnimation()
    {
        anim.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        anim.SetFloat("Vertical", Input.GetAxis("Vertical") * (Input.GetButton("Sprint") ? 2f : 1f));
        anim.SetBool("jump", controller.isGrounded && !Input.GetButton("Jump"));
    }

    private void OnChangeHealth(float oldHealth, float newhealth)
    {
        if (isLocalPlayer)
        {
            float value = 1 - (this.currenthealth / this.maxhealth);
            sO.color = new Color(1, 1, 1, value >= 0.6f ? ((value - 0.6f) / 0.4f) : 0);
            if (oldHealth > newhealth)
            {
                StartCoroutine(hitDamage());
            }
            if (value < 0.6f)
            {
                heartAudioS.Stop();
            }
            else
            {
                heartAudioS.Play();
                heartAudioS.pitch = (((value - 0.6f) / 0.4f) * 0.3f) + 1;
                heartAudioS.volume = ((value - 0.6f) / 0.4f) * 0.3f;
            }
        }
    }

    private void OnChangeName(string oldName, string newName)
    {
        textName.text = this.namePL;
    }

    IEnumerator hitDamage()
    {
        if (!this.isHit)
        {
            this.isHit = true;
            float timer = degatTime;
            if (mouseAudioS)
            {
                mouseAudioS.clip = hurtDegat[Random.Range(0, hurtDegat.Length)];
                mouseAudioS.PlayOneShot(mouseAudioS.clip);
            }
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                sOHit.color = new Color(1, 0, 0, (timer / this.degatTime));
                yield return null;
            }
            yield return new WaitForSeconds(degatTime);
            this.isHit = false;
        }
    }

    [Server]
    IEnumerator RegenHealth()
    {
        while (isAlive)
        {
            if (this.maxhealth < this.currenthealth + regenneHealth)
            {
                if (this.currenthealth != this.maxhealth)
                {
                    this.currenthealth = this.maxhealth;
                }
            }
            else
            {
                this.currenthealth += regenneHealth;
            }
            yield return new WaitForSeconds(regenneRate);
        }
    }

    public void Receiveheal(float heal)
    {
        if (isServer)
        {
            if (this.currenthealth + heal > this.maxhealth)
            {
                this.currenthealth = this.maxhealth;
            }
            else
            {
                this.currenthealth += heal;
            }
            this.isAlive = true;
        }
    }

    private void OnChangeAlive(bool oldLive, bool newLive)
    {
        if (isLocalPlayer)
        {
            sO.gameObject.SetActive(isAlive);
            sOHit.gameObject.SetActive(isAlive);
            foreach (GameObject oe in objectToEnabled)
            {
                oe.SetActive(!isAlive);
            }
            foreach (GameObject od in objectToDisabled)
            {
                od.SetActive(isAlive);
            }
            foreach (Behaviour b in scriptToDisabled)
            {
                b.enabled = isAlive;
            }
            if (isAlive)
            {
                foreach (GameObject h in head)
                {
                    h.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
            else
            {
                foreach (GameObject h in head)
                {
                    h.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                torse.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
            gameObject.GetComponent<WeaponManager>().SetDeath(isAlive);
            anim.SetBool("Dead", !isAlive);
        }
    }

    public void ReceiveDamage(float damage)
    {
        if (isServer)
        {
            if (this.currenthealth - damage <= 0)
            {
                this.currenthealth = 0;
                this.isAlive = false;
            }
            else
            {
                this.currenthealth -= damage;
            }
        }
    }

    public bool GetIsAlive()
    {
        return this.isAlive;
    }
}
