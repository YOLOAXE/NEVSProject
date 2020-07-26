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
        [SerializeField] private float multAmmo = 1.0f;
        [Header("Shoot Setting")]
        [SerializeField] private float forceSendGrenade = 3000f;
        [SerializeField] private GameObject grenadeSpawn = null;
        [SerializeField] private GameObject targetCamera = null;
        private bool isShoot = false;

        public override IEnumerator shoot()
        {
            if (this.currentMunition > 0)
            {
                if (!isShoot && GameInputManager.GetKeyDown("Tire"))
                {
                    isShoot = true;
                    base.netAnim.SetTrigger("shootOneShot");
                    yield return new WaitForSeconds(this.shootRate);
                    base.wM.CmdTire();
                    isShoot = false;
                }
            }
            else if (GameInputManager.GetKeyDown("Tire"))
            {
                base.netAnim.SetTrigger("noAmmo");
            }
            yield return null;
        }

        public override void OnChangeCM(int mun, int charg, bool draw)
        {
            this.currentMunition = mun;
            m_animator.SetBool("reload", mun <= 0);
            if (draw)
            {
                base.wM.SetTextMun(this.currentMunition.ToString());
            }
        }

        public override bool AddMunition(int munCharg)
        {
            this.currentMunition += (int)(munCharg * multAmmo);
            base.wM.RpcSendMunition(base.idArme, this.currentMunition, 0);
            return true;
        }

        public override int GetMultByAddMun(int munCharg)
        {
            return (int)(munCharg * multAmmo);
        }

        public override void CmdSendTire()
        {
            if (currentMunition > 0)
            {
                GameObject io = Instantiate(grenadeSpawn, transform.position, Quaternion.identity);
                io.GetComponent<Rigidbody>().AddForce(this.targetCamera.transform.TransformDirection(Vector3.forward) * forceSendGrenade);
                NetworkServer.Spawn(io);
                io.GetComponent<GrenadeExplosion>().SetOwner(base.wM.gameObject);
                this.currentMunition--;
                base.wM.RpcSendMunition(base.idArme, this.currentMunition, 0);
            }
        }

        public override void OnSelectWeapon()
        {
            base.wM.SetTextMun(this.currentMunition.ToString());
            m_animator.SetBool("reload", this.currentMunition <= 0);
        }

        public override void OnChangeWeapon()
        {
            base.wM.SetTextMun("");
            m_animator.SetBool("reload", false);
        }
    }
}