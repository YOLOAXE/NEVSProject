using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace VHS
{
    public class Player : NetworkBehaviour
    {
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
        [SerializeField] private bool isAlive = true;
        [Header("Audio")]
        [SerializeField] private AudioSource mouseAudioS = null;
        [SerializeField] private AudioSource heartAudioS = null;
        [SerializeField] private AudioClip[] hurtDegat = null;

        void Start()
        {
            sO = GameObject.Find("SangOverlay").GetComponent<Image>();
            sOHit = GameObject.Find("SangOverlay2").GetComponent<Image>();
        }

        void Update()
        {
            if(!isLocalPlayer)
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

        private void OnChangeHealth(float oldHealth,float newhealth)
        {
            float value = 1-(this.currenthealth / this.maxhealth);
            sO.color = new Color(1, 1, 1, value >= 0.6f ? ((value-0.6f)/0.4f) : 0);
            if (oldHealth > newhealth)
            {
                StartCoroutine(hitDamage());
            }
            if(value < 0.6f)
            {
                heartAudioS.Stop();
            }
            else
            {
                heartAudioS.Play();
                heartAudioS.pitch = (((value - 0.6f) / 0.4f)*0.3f) + 1;
                heartAudioS.volume = ((value - 0.6f) / 0.4f)*0.3f;
            }
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
                while(timer > 0)
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
            while(isAlive)
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
        public void ReceiveDamage(float damage)
        {
            if (isServer)
            {
                this.currenthealth -= damage;
            }
        }
    }
}