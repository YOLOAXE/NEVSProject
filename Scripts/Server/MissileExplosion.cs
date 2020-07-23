using UnityEngine;
using Mirror;
using System.Collections.Generic;
using VHS;

namespace DitzelGames.FastIK
{
    public class MissileExplosion : NetworkBehaviour
    {
        [Header("Missile Setting")]
        [SerializeField] private float destroyAfter = 7f;
        [SerializeField] private float explodeAfter = 8f;
        [SerializeField] private float explosionDegatBase = 300f;
        [SerializeField] private float explosionForce = 800f;
        [SerializeField] private float explosionRadius = 10f;
        [SerializeField] private float ShakeRadius = 40f;
        [SerializeField] private float shakeDuration = 0.1f;
        [SerializeField] private float shakeMagnitude = 0.1f;
        [SerializeField] private float speedRocket = 2f;
        [SerializeField] private ParticleSystem ps = null;
        [SerializeField] private AudioSource m_audio = null;
        [SerializeField] private AudioClip m_clip = null;
        [SerializeField] private MeshRenderer[] partMissile = null;
        private Rigidbody rb = null;
        private GameObject owner = null;
        [SyncVar(hook = "PlayExplosionAndSound")] private bool hasExplose = false;
        [Header("After Explosion")]
        [SerializeField] private AudioSource missileAudio = null;

        void PlayExplosionAndSound(bool oldH,bool newH)
        {
            if (this.m_audio)
            {
                this.m_audio.clip = this.m_clip;
                this.m_audio.PlayOneShot(this.m_audio.clip);
            }
            this.ps.Play();
            foreach (MeshRenderer m in this.partMissile)
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
            if(missileAudio)
            {
                missileAudio.Stop();
            }
        }

        public void SetOwner(GameObject player)
        {
            this.owner = player;
        }
        #region Start & Stop Callbacks

        [Server]
        void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Server]
        void ExplosionPhysicsDamage()
        {
            hasExplose = true;
            Invoke(nameof(DestroySelf), destroyAfter);
            GetComponent<Rigidbody>().isKinematic = true;
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider nerObject in colliders)
            {
                Rigidbody rb = nerObject.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.AddExplosionForce(this.explosionForce, transform.position, this.explosionRadius);
                }
                EnemieMembre ne = nerObject.GetComponent<EnemieMembre>();
                if (ne)
                {
                    ne.ReceiveDamageMembre(explosionDegatBase * (1 - (Vector3.Distance(transform.position, nerObject.transform.position) / explosionRadius)), this.owner);
                }
            }
        }

        void Start()
        {
            if (!isServer) { return; }
            rb = GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * speedRocket, ForceMode.Impulse);
            Invoke(nameof(ExplosionPhysicsDamage), explodeAfter);
        }

        void Update()
        {
            if (!isServer) { return; }
            rb.AddForce(transform.forward * speedRocket * Time.deltaTime);
        }

        [ServerCallback]
        void OnTriggerStay(Collider other)
        {            
            if (!hasExplose && !(other.transform.tag == "nlPlayer" || other.transform.tag == "Player"))
            {
                hasExplose = true;
                ExplosionPhysicsDamage();
            }
        }

        [ServerCallback]
        void OnCollisionStay(Collision collision)
        {
            if(!hasExplose && !(collision.transform.tag == "nlPlayer" ||  collision.transform.tag == "Player"))
            {
                hasExplose = true;
                ExplosionPhysicsDamage();
            }
        }
        #endregion
    }
}
