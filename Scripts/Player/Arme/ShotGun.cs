using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace DitzelGames.FastIK
{
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

    public class ShotGun : AArme
    {
        [Header("Arme Setting")]
        [SerializeField] private Animator m_animator = null;
        [SerializeField] private WeaponManager wM = null;
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
        [Header("ShotGunEffect")]
        [SerializeField] private impactEffect[] ie = null;
        private bool isShoot = false;
        private bool isReload = false;

        public override IEnumerator shoot()
        {
            if (!isShoot && !isReload && Input.GetButtonDown("Fire1"))
            {
                isShoot = true;
                m_animator.SetBool("shootOneShot",true);
                CmdSendTire();
                yield return new WaitForSeconds(this.shootRate);
                isShoot = false;
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
            bool tagAply = false;
            for (byte i = 0; i < this.balleparTire;i++)
            {
                Vector3 direction = Random.insideUnitCircle * this.scale;
                direction.z = this.Z;
                direction = this.targetCamera.transform.TransformDirection(direction.normalized);
                this.ray = new Ray(this.targetCamera.transform.position, direction);
                if (Physics.Raycast(this.ray, out hit, Mathf.Infinity, this.layerImpactDegat))
                {
                    for(byte j = 0; j < ie.Length;j++)
                    {
                        if(ie[i].getTag() == hit.transform.tag)
                        {
                            GameObject io = Instantiate(ie[i].getImpact(), hit.point, Quaternion.identity);
                            //GameObject go = Instantiate(otherPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                            //NetworkServer.Spawn(go, connectionToClient);
                            tagAply = true;
                        }
                    }
                    if(tagAply)
                    {

                    }
                }
            }
            this.currentMunition--;
        }
    }
}