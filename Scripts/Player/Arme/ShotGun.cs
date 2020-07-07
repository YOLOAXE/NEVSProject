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
        [SerializeField] private int currentMunition = 0;
        [SerializeField] private int chargeurMunition = 0;
        [SerializeField] private int maxMunition = 0;
        [SerializeField] private float shootRate = 0.8f;
        [SerializeField] private float reloadTimeParDouille = 0.8f;
        [SerializeField] private float magnitudeShakeShoot = 0.1f;
        [SerializeField] private float timeShakeShoot = 0.5f;
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
                    base.netAnim.SetTrigger("shootOneShot");
                    base.wM.CmdTire();
                    base.wM.StartcShake(this.magnitudeShakeShoot, this.timeShakeShoot);
                    yield return new WaitForSeconds(this.shootRate);
                    isShoot = false;
                }
            }
            else if(Input.GetButtonDown("Fire1"))
            {
                base.netAnim.SetTrigger("noAmmo");
            }
            yield return null;
        }

        public override IEnumerator reload()
        {
            if (this.chargeurMunition > 0 && this.currentMunition < this.maxMunition && !isReload)
            {
                isReload = true;
                m_animator.SetBool("reload", true);
                base.wM.CmdReload();
            }
            yield return null;
        }

        public override void CmdSendTire()
        {
            if (currentMunition > 0 && !isReload)
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
                        //io.transform.parent = hit.transform;
                        if (hit.rigidbody)
                        {
                            hit.rigidbody.AddForce(this.ray.direction * hitForceTire);
                        }
                        NetworkServer.Spawn(io);
                    }
                }
                this.currentMunition--;
                base.wM.RpcSendMunition(this.currentMunition,this.chargeurMunition);
            }
        }

        public override IEnumerator CmdSendReload()
        {
            isReload = true;
            for (int i = 0; i < this.maxMunition && this.chargeurMunition > 0 && this.currentMunition < this.maxMunition; i++)
            {
                yield return new WaitForSeconds(reloadTimeParDouille);
                this.currentMunition++;
                this.chargeurMunition--;
                base.wM.RpcSendMunition(this.currentMunition, this.chargeurMunition);
            }
            yield return new WaitForSeconds(reloadTimeParDouille);
            isReload = false;
        }

        public override void OnChangeCM(int mun, int charg)
        {
            this.chargeurMunition = charg;
            this.currentMunition = mun;
            base.wM.SetTextMun(this.currentMunition.ToString() + "/" + this.chargeurMunition.ToString());
            if (this.chargeurMunition == 0 || this.currentMunition == this.maxMunition)
            {
                this.isReload = false;
                m_animator.SetBool("reload", false);
            }
        }

        public override void OnSelectWeapon()
        {
            base.wM.SetTextMun(this.currentMunition.ToString() + "/" + this.chargeurMunition.ToString());
        }

        public override void OnChangeWeapon()
        {
            base.wM.SetTextMun("");
        }
    }
}