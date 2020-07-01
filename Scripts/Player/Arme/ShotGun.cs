using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace DitzelGames.FastIK
{
    public class ShotGun : AArme
    {
        [Header("Arme Setting")]
        [SerializeField] private Animator m_animator = null;
        [SerializeField] [SyncVar] private int currentMunition = 0;
        [SerializeField] [SyncVar] private int chargeurMunition = 0;
        [SerializeField] private int maxMunition = 0;
        [SerializeField] private float shootRate = 0.8f;
        [Header("Arme Raycast")]
        [SerializeField] private LayerMask layerImpactDegat = 0;
        [SerializeField] private float hitForceTire = 0;
        [SerializeField] private int balleparTire = 10;
        [SerializeField] private float degatParBalle = 10.0f;
        [SerializeField] private float Z = 10;
        [SerializeField] private float scale = 0.1f;
        [SerializeField] private GameObject targetCamera = null;
        private RaycastHit hit;
        private Ray ray;
        private bool isShoot = false;
        private bool isReload = false;

        public override IEnumerator shoot()
        {
            if (currentMunition > 0)
            {
                if (!isShoot && !isReload && Input.GetButtonDown("Fire1"))
                {
                    isShoot = true;
                    m_animator.SetBool("shootOneShot", true);
                    base.wM.CmdTire();
                    yield return new WaitForSeconds(this.shootRate);
                    isShoot = false;
                }
            }
            else if(Input.GetButtonDown("Fire1"))
            {
                m_animator.SetBool("noAmmo", true);
            }
            yield return null;
        }

        public override IEnumerator reload()
        {
            if (!isReload)
            {
                isReload = true;
                yield return null;
                isReload = false;
            }
            yield return null;
        }

        public override void CmdSendTire()
        {
            if (currentMunition > 0)
            {
                for (byte i = 0; i < this.balleparTire; i++)
                {
                    Vector3 direction = Random.insideUnitCircle * this.scale;
                    direction.z = this.Z;
                    direction = this.targetCamera.transform.TransformDirection(direction.normalized);
                    this.ray = new Ray(this.targetCamera.transform.position, direction);
                    if (Physics.Raycast(this.ray, out hit, Mathf.Infinity, this.layerImpactDegat))
                    {
                        GameObject io = Instantiate(base.wM.getImpactByTag(hit.transform.tag), hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                        NetworkServer.Spawn(io);
                    }
                }
                this.currentMunition--;
            }
        }
    }
}