using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using VHS;
using Mirror.Examples.MultipleAdditiveScenes;

public class Boss : NetworkBehaviour
{
    [Header("Boss Setting")]
    [SerializeField] private float currentHealth = 100.0f;
    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private bool dead = false;
    [SerializeField] private bool inFight = false;
    [SerializeField] private bool inIntro = false;
    [SerializeField] private float tempsIntro = 10f;
    [Header("Deplacement")]
    [SerializeField] private GameObject tpPlayerPoint = null;
    [SerializeField] private List<GameObject> AllPlayer = null;
    [Header("Attaque zone")]
    [SerializeField] private AttaqueBoss[] ab = null;
    [SerializeField] private int indiceAB = -1;
    [SerializeField] private bool inAttaque = false;
    [SerializeField] private float tempsNextAttaque = 5f;
    private float timerNextAttaque = 0f;
    [Header("Animator")]
    [SerializeField] private NetworkAnimator na = null;
    [Header("Dead")]
    [SerializeField] private deathBoss db = null;

    #region Start & Stop Callbacks
    [Server]
    public void GetAllPlayer()
    {
        this.AllPlayer.Clear();
        this.AllPlayer = GameObject.Find("NetworkManager").GetComponent<GameNetworkManager>().GetPlayerList();        
    }

    public void StartFight()
    {
        GetAllPlayer();
        this.RpcTpAllPlayer();
        this.inIntro = true;
        this.inFight = true;       
        na.SetTrigger("Intro");
        StartCoroutine(IntroWait());
        StartCoroutine(tpb());
    }

    void Update()
    {
        if (!isServer || !inFight || inIntro) { return; }
        attaque();
    }

    [Server]
    void attaque()
    {
        if(ab.Length > 0)
        {
            if (!inAttaque)
            {
                if (timerNextAttaque <= 0)
                {
                    inAttaque = true;
                    int rd = Random.Range(0, ab.Length);
                    if(this.indiceAB == rd){ rd = (rd + 1)%ab.Length;}
                    this.indiceAB = rd; 
                    ab[this.indiceAB].attaqueStart();
                }
                timerNextAttaque -= Time.deltaTime;
            }
            else
            {
                ab[this.indiceAB].attaqueUpdate();
                ab[this.indiceAB].deplacement();
            }
        }
    }

    public void EndAttaque()
    {
        timerNextAttaque = tempsNextAttaque;
        inAttaque = false;
        if(dead)
        {
            inFight = false;
            db.isDead();
        }
    }
    #endregion

    IEnumerator IntroWait()
    {
        RpcShakeAllPlayer(0.8f,0.3f);
        yield return new WaitForSeconds(tempsIntro);
        this.inIntro = false;
    }

    IEnumerator tpb()
    {
        float timer = 1.0f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            foreach (GameObject p in this.AllPlayer)
            {
                p.transform.position = tpPlayerPoint.transform.position;
            }
            yield return null;
        }
    }

    [ClientRpc]
    private void RpcTpAllPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            player.transform.position = tpPlayerPoint.transform.position;
        }
    }

    public void AplyTriggerNe(string trig)
    {
        na.SetTrigger(trig);
    }

    public void ReceiveDamage(float Damage)
    {
        if (!inFight) { return; }
        this.currentHealth -= Damage;
        if(this.currentHealth <= 0)
        {
            dead = true;
        }
    }
    [ClientRpc]
    public void RpcShakeAllPlayer(float duration, float magnitude)
    {
        GameObject.FindWithTag("Player").GetComponent<WeaponManager>().StartcShake(duration, magnitude);
    }
}
