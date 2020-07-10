using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using VHS;

namespace DitzelGames.FastIK
{
    public class Seringue : AArme
    {
        [Header("Soin Setting")]
        [SerializeField] private int activePourcentage = 0;
        [SerializeField] private float increasePRate = 0.8f;
        [SerializeField] private bool drawtxt = false;
        [SerializeField] private float healing = 60f;
        [Header("Soin Overlaps")]
        [SerializeField] private float radiusSoin = 2f;

        void Start()
        {
            StartCoroutine(Regenne());
        }

        public override IEnumerator shoot()
        {
            if (this.activePourcentage >= 100 && Input.GetButtonDown("Fire1"))
            {
                GameObject Target = GetPlayerSoin();
                if (Target)
                {
                    base.netAnim.SetTrigger("shootOneShot");
                    base.wM.CmdTire();
                }
                else
                {
                    base.netAnim.SetTrigger("reloadOneShot");
                }
                yield return null;
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                base.netAnim.SetTrigger("reloadOneShot");
            }
        }

        public override void CmdSendTire()
        {
            GameObject Target = GetPlayerSoin();
            if (Target)
            {
                if (activePourcentage >= 100)
                {
                    this.activePourcentage = 0;
                    Target.GetComponent<Player>().Receiveheal(healing);
                    base.wM.RpcSendMunition(base.idArme, this.activePourcentage, 0);
                }
            }
        }

        private GameObject GetPlayerSoin()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, this.radiusSoin);
            foreach (Collider c in colliders)
            {
                if (c.transform.tag == "Player" || c.transform.tag == "nlPlayer")
                {
                    if (c.gameObject != base.wM.gameObject)
                    {
                        return c.gameObject;
                    }
                }
            }
            return null;
        }

        IEnumerator Regenne()
        {
            while (true)
            {
                if (this.activePourcentage < 100)
                {
                    this.activePourcentage++;
                    if (drawtxt)
                    {
                        base.wM.SetTextMun(this.activePourcentage.ToString() + "%");
                    }
                }
                if (isServer && this.activePourcentage % 10 == 0)
                {
                    base.wM.RpcSendMunition(base.idArme, this.activePourcentage, 0);
                }
                yield return new WaitForSeconds(this.increasePRate);
            }

            yield return null;
        }

        public override void OnChangeCM(int mun, int charg, bool draw)
        {
            this.activePourcentage = mun;
            if (draw)
            {
                base.wM.SetTextMun(this.activePourcentage.ToString() + "%");
            }
        }

        public override void OnSelectWeapon()
        {
            drawtxt = true;
            StartCoroutine(Regenne());
            base.wM.SetTextMun(this.activePourcentage.ToString() + "%");
        }

        public override void OnChangeWeapon()
        {
            drawtxt = false;
            base.wM.SetTextMun("");
        }

    }
}
