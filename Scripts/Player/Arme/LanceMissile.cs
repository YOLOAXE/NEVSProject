using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace DitzelGames.FastIK
{
    public class LanceMissile : AArme
    {
        [Header("Arme Setting")]
        [SerializeField] private Animator m_animator = null;
        [SerializeField] private int currentMunition = 1;
        [SerializeField] private int chargeurMunition = 10;
        [SerializeField] private int maxMunition = 1;
        [SerializeField] private float shootRate = 1.2f;
        [SerializeField] private float reloadTime = 1.5f;
        [Header("Shoot Setting")]
        [SerializeField] private GameObject spawnPoint = null;
        [SerializeField] private GameObject projectilSpawn = null;
        [SerializeField] private GameObject targetCamera = null;
        [SerializeField] private bool isShoot = false;
        [SerializeField] private bool isReload = false;

        public override IEnumerator shoot()
        {
            if (this.currentMunition > 0)
            {
                if (!isShoot && Input.GetButtonDown("Fire1") && !this.isReload)
                {
                    isShoot = true;
                    base.netAnim.SetTrigger("shootOneShot");
                    base.wM.CmdTire();
                    yield return new WaitForSeconds(this.shootRate);
                    isShoot = false;
                }
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                base.netAnim.SetTrigger("noAmmo");
            }
            yield return null;
        }

        public override IEnumerator reload()
        {
            if (this.chargeurMunition > 0 && this.currentMunition < this.maxMunition && !isReload)
            {
                spawnPoint.GetComponent<MeshRenderer>().enabled = false;
                isReload = true;
                base.netAnim.SetTrigger("reloadOneShot");
                base.wM.CmdReload();
            }
            yield return null;
        }

        public override IEnumerator CmdSendReload()
        {
            isReload = true;
            for (int i = 0; i < this.maxMunition && this.chargeurMunition > 0 && this.currentMunition < this.maxMunition; i++)
            {
                yield return null;
                this.currentMunition++;
                this.chargeurMunition--;                
            }
            yield return new WaitForSeconds(this.reloadTime);
            base.wM.RpcSendMunition(base.idArme, this.currentMunition, this.chargeurMunition);
            isReload = false;
        }

        public override void OnChangeCM(int mun, int charg, bool draw)
        {
            this.currentMunition = mun;
            this.chargeurMunition = charg;
            if (draw)
            {
                base.wM.SetTextMun(this.chargeurMunition.ToString());
            }
            if (this.chargeurMunition == 0 || this.currentMunition == this.maxMunition)
            {
                spawnPoint.GetComponent<MeshRenderer>().enabled = true;
                this.isReload = false;
            }
            else
            {
                StartCoroutine(reload());
            }
            if(this.currentMunition == 0)
            {
                spawnPoint.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        public override void CmdSendTire()
        {
            if (currentMunition > 0 && !isReload)
            {
                GameObject io = Instantiate(projectilSpawn, spawnPoint.transform.position, Quaternion.LookRotation(this.targetCamera.transform.TransformDirection(Vector3.forward)));
                NetworkServer.Spawn(io);
                io.GetComponent<MissileExplosion>().SetOwner(base.wM.gameObject);
                this.currentMunition--;
                base.wM.RpcSendMunition(base.idArme, this.currentMunition, this.chargeurMunition);
            }
        }

        public override void OnSelectWeapon()
        {
            base.wM.SetTextMun(this.chargeurMunition.ToString());
            if (this.currentMunition <= 0)
            {
                base.netAnim.SetTrigger("reloadOneShot");
            }
            if (this.currentMunition == 0)
            {
                spawnPoint.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        public override void OnChangeWeapon()
        {
            base.wM.SetTextMun("");
            m_animator.SetBool("reloadOneShot", false);
        }
    }
}