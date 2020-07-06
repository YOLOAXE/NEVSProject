using UnityEngine;
using Mirror;
using System.Collections.Generic;

namespace DitzelGames.FastIK
{
    public class GrenadeExplosion : NetworkBehaviour
    {
        [Header("Grenade Setting")]
        [SerializeField] private float destroyAfter = 15f;
        [SerializeField] private float explodedAfter = 10f;
        [SerializeField] private float explosionDegatBase = 300f;
        [SerializeField] private float explosionForce = 800f;
        [SerializeField] private float explosionRadius = 10f;
        [SerializeField] private float ShakeRadius = 40f;
        [SerializeField] private float shakeDuration = 0.1f;
        [SerializeField] private float shakeMagnitude = 0.1f;
        [SerializeField] private ParticleSystem ps = null;
        [SerializeField] private AudioSource m_audio = null;
        [SerializeField] private AudioClip m_clip = null;
        [SerializeField] private MeshRenderer[] partGrenade = null;

        void Start()
        {
            Invoke(nameof(PlayExplosionAndSound), explodedAfter);
        }

        void PlayExplosionAndSound()
        {
            if (this.m_audio)
            {
                this.m_audio.clip = this.m_clip;
                this.m_audio.PlayOneShot(this.m_audio.clip);
            }
            this.ps.Play();
            foreach (MeshRenderer m in this.partGrenade)
            {
                m.enabled = false;
            }
            Collider[] colliders = Physics.OverlapSphere(transform.position, this.ShakeRadius);
            foreach (Collider nerObject in colliders)
            {
                if (nerObject.transform.tag == "Player")
                {
                    nerObject.GetComponent<WeaponManager>().StartcShake(this.shakeDuration, this.shakeMagnitude);
                }
            }
        }
        #region Start & Stop Callbacks

        public override void OnStartServer()
        {
            Invoke(nameof(ExplosionPhysicsDamage), explodedAfter);
            Invoke(nameof(DestroySelf), destroyAfter);
        }

        [Server]
        void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Server]
        void ExplosionPhysicsDamage()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider nerObject in colliders)
            {
                Rigidbody rb = nerObject.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.AddExplosionForce(this.explosionForce, transform.position, this.explosionRadius);
                }
            }
        }

        #endregion
    }
}
