using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class deathBoss : NetworkBehaviour
{
    [SerializeField] [SyncVar(hook = "OnChangeDeath")] private bool isdead = false;
    [SerializeField] private Boss b = null;
    [SerializeField] private float movementSpeed = 15f;
    [SerializeField] private AnimationCurve curveDeplacement;
    [SerializeField] private GameObject barriere = null;
    private Vector3 departPosition = new Vector3(0, 0, 0);
    private float dist = 0;
    private bool endBoss = false;
    [Header("Audio Setting")]
    [SerializeField] private AudioSource m_audio = null;

    void Update()
    {
        if (!isdead && !endBoss) { return; }
        dist = Vector3.Distance(departPosition, b.transform.position);
        b.transform.position = Vector3.MoveTowards(b.transform.position, transform.position, movementSpeed * Time.deltaTime * curveDeplacement.Evaluate(dist / Vector3.Distance(departPosition, transform.position)));
        if(dist <= 0.5f)
        {
            endBoss = true;
            b.AplyTriggerNe("dead");
        }
    }

    public void isDead()
    {
        departPosition = b.transform.position;        
        isdead = true;
    }

    private void OnChangeDeath(bool OldD,bool NewD)
    {
        if(this.isdead)
        {
            StartCoroutine(AudioChanger());
        }
    }

    IEnumerator AudioChanger()
    {
        while(m_audio.volume > 0)
        {
            m_audio.volume -= (Time.deltaTime/3);
            yield return null;
        }
        barriere.SetActive(false);
        yield return null;
    }
}
