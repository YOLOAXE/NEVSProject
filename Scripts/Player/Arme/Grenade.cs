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
        [SerializeField] private int currentMunition = 10;
        [SerializeField] private float shootRate = 0.8f;
        [Header("Shoot Setting")]
        [SerializeField] private float forceSendGrenade = 3000f;
        [SerializeField] private GameObject grenadeSpawn = null;
        [SerializeField] private GameObject targetCamera = null;
        private bool isShoot = false;

        public override IEnumerator shoot()
        {
            if (this.currentMunition > 0)
            {
                if (!isShoot && Input.GetButtonDown("Fire1"))
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

        public override void OnChangeCM(int mun, int charg)
        {
            this.currentMunition = mun;
            base.wM.SetTextMun(this.currentMunition.ToString());
        }

        public override void CmdSendTire()
        {
            if (currentMunition > 0)
            {
                GameObject io = Instantiate(grenadeSpawn, transform.position, Quaternion.identity);
                io.GetComponent<Rigidbody>().AddForce(this.targetCamera.transform.TransformDirection(Vector3.forward) * forceSendGrenade);
                NetworkServer.Spawn(io);
                this.currentMunition--;
                base.wM.RpcSendMunition(this.currentMunition, 0);
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