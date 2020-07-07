using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace DitzelGames.FastIK
{
    public class Sniper : AArme
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
        [SerializeField] private float degatParBalle = 10.0f;
        [SerializeField] private float Z = 10;
        [SerializeField] private float scale = 0.1f;
        private RaycastHit hit;
        private Ray ray;
        private bool isShoot = false;
        private bool isReload = false;
        [Header("ArmeAIM")]
        [SerializeField] private GameObject cameraAIM = null;
        [SerializeField] private GameObject ScopeOverlay = null;
        [SerializeField] private bool bAim = false;
        private float timeToAim = 0.3f;

        public override void OnSelectWeapon()
        {
            ScopeOverlay = GameObject.Find("AimScopeOverlay");
            base.wM.SetTextMun(this.currentMunition.ToString() + "/" + this.chargeurMunition.ToString());
        }

        public override IEnumerator shoot()
        {
            if (this.currentMunition > 0)
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
                Vector3 direction = Random.insideUnitCircle * this.scale;
                direction.z = this.Z;
                direction = this.cameraAIM.transform.TransformDirection(direction.normalized);
                this.ray = new Ray(this.cameraAIM.transform.position, direction);
                if (Physics.Raycast(this.ray, out hit, Mathf.Infinity, this.layerImpactDegat))
                {
                    GameObject io = Instantiate(base.wM.getImpactByTag(hit.transform.tag), hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    if (hit.rigidbody)
                    {
                        hit.rigidbody.AddForce(this.ray.direction * hitForceTire);
                    }
                    NetworkServer.Spawn(io);
                    io.GetComponent<ImpactGlue>().AplyGlue(hit.transform.gameObject);
                }
                this.currentMunition--;
                base.wM.RpcSendMunition(base.idArme, this.currentMunition, this.chargeurMunition);
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
                base.wM.RpcSendMunition(base.idArme, this.currentMunition, this.chargeurMunition);
            }
            yield return new WaitForSeconds(reloadTimeParDouille);
            isReload = false;
        }

        public override void OnChangeCM(int mun, int charg,bool draw)
        {
            this.chargeurMunition = charg;
            this.currentMunition = mun;
            if (draw)
            {
                base.wM.SetTextMun(this.currentMunition.ToString() + "/" + this.chargeurMunition.ToString());
            }
            if (this.chargeurMunition == 0 || this.currentMunition == this.maxMunition)
            {
                this.isReload = false;
                m_animator.SetBool("reload", false);
            }
        }

        public override void AimArme(bool state)
        {
            if(this.bAim != state)
            {
                timeToAim = 0.3f;
            }
            this.bAim = state;
            if (timeToAim > 0 && this.bAim || this.isReload)
            {
                timeToAim -= Time.deltaTime;
            }
            else
            {
                cameraAIM.SetActive(this.bAim);
                if (ScopeOverlay)
                {
                    ScopeOverlay.GetComponent<Image>().enabled = this.bAim;
                }
            }
        }

        public override void OnChangeWeapon()
        {
            this.bAim = false;
            cameraAIM.SetActive(this.bAim);
            base.wM.SetTextMun("");
            if (ScopeOverlay)
            {
                ScopeOverlay.GetComponent<Image>().enabled = false;
            }
        }
    }
}