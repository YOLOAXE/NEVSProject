using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace DitzelGames.FastIK
{
    public class Grenade : AArme
    {
        [Header("Arme Setting")]
        [SerializeField] private Animator m_animator = null;
        [SerializeField] [SyncVar(hook = nameof(OnChangeMunition))] private int currentMunition = 0;
        [SerializeField] private int maxMunition = 0;
        [SerializeField] private float shootRate = 0.8f;
        [Header("Shoot Setting")]
        [SerializeField] private float forceSendGrenade = 3000f;
        [SerializeField] private GameObject grenadeSpawn = null;
        [SerializeField] private GameObject spawnPoint = null;
        [SerializeField] private GameObject targetCamera = null;
        private bool isShoot = false;

        public override IEnumerator shoot()
        {
            if (this.currentMunition > 0)
            {
                if (!isShoot && Input.GetButtonUp("Fire1"))
                {
                    isShoot = true;
                    base.netAnim.SetTrigger("shootOneShot");
                    base.wM.CmdTire();
                    yield return new WaitForSeconds(this.shootRate);
                    isShoot = false;
                }
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                base.netAnim.SetTrigger("noAmmo");
            }
            yield return null;
        }

        private void OnChangeMunition(int newCM, int oldCM)
        {
            base.wM.SetTextMun(this.currentMunition.ToString());
        }

        public override void CmdSendTire()
        {
            if (currentMunition > 0)
            {
                GameObject io = Instantiate(grenadeSpawn, spawnPoint.transform.position, Quaternion.identity);
                io.GetComponent<Rigidbody>().AddForce(this.targetCamera.transform.TransformDirection(Vector3.forward) * forceSendGrenade);
                this.currentMunition--;
            }
        }

        public override void OnSelectWeapon()
        {
            base.wM.SetTextMun(this.currentMunition.ToString());
        }

        public override void OnChangeWeapon()
        {
            base.wM.SetTextMun("");
        }
    }
}