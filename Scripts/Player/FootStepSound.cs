using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class audioClipStepTag
{
    [SerializeField] private string tagName = "";
    [SerializeField] private AudioClip[] stepSound = null;

    public string getTagName()
    {
        return this.tagName;
    }

    public AudioClip getClipStep(int indice)
    {
        return this.stepSound[indice];
    }
    public int getLength()
    {
        return this.stepSound.Length;
    }
};

public class FootStepSound : NetworkBehaviour
{
    [SerializeField] private AudioSource m_audioSource = null;
    [SerializeField] private audioClipStepTag[] acst = null;
    [SerializeField] private AudioClip jump = null;
    [SerializeField] private LayerMask lm = 0;
    private RaycastHit hit;

    public void AplyStep(int idDirection)
    {
        if(Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0 && idDirection == 1 && !Input.GetButton("Sprint"))
        {
            return;
        }
        if(Physics.Raycast(transform.position,-transform.up,out hit,lm))
        {
            foreach(audioClipStepTag a in acst)
            {
                if(a.getTagName() == hit.transform.tag)
                {
                    this.m_audioSource.clip = a.getClipStep(Random.Range(0, a.getLength()));
                    this.m_audioSource.PlayOneShot(this.m_audioSource.clip);
                    return;
                }
            }
            if(acst.Length > 0)
            {
                this.m_audioSource.clip = acst[0].getClipStep(Random.Range(0, acst[0].getLength()));
                this.m_audioSource.PlayOneShot(this.m_audioSource.clip);
            }
        }
    }

    public void AplyJump()
    {
        this.m_audioSource.clip = this.jump;
        this.m_audioSource.PlayOneShot(this.m_audioSource.clip);
    }
}
