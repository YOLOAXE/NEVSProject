using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

namespace VHS
{
    public class Zombie : NavEnemie
    {

        [Header("Attaque Setting")]
        [SerializeField] private GameObject shootPoint = null;
        [SerializeField] private bool attaque = false;
        [SerializeField] private float degatZombie = 23f;
        private float distance = 0f;
        [Header("Attaque Overlaps")]
        [SerializeField] private float radiusAttaque = 1f;
        [SerializeField] private float attaqueRate = 1f;
        Vector3 lookPos = Vector3.zero;
        Quaternion lookAngle = Quaternion.identity;

        #region Start & Stop Callbacks
        public override void attack()
        {
            base.agent.isStopped = true;
            if (base.targetPlayer)
            {
                if (Vector3.Distance(this.transform.position, base.targetPlayer.transform.position) > base.huntingDistance)
                {
                    base.agent.SetDestination(base.targetPlayer.transform.position);
                    base.agent.isStopped = false;
                }
                else
                {
                    this.shootPoint.transform.LookAt(base.targetPlayer.transform.position + new Vector3(0f, 0.5f, 0f));
                    StartCoroutine(ShootIEnum());
                }
            }

            lookPos = this.targetPlayer.transform.position - this.transform.position;
            lookPos.y = 0;
            lookAngle = Quaternion.LookRotation(lookPos);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, lookAngle, Time.deltaTime * (base.agent.angularSpeed/30));
        }

        #endregion

        IEnumerator ShootIEnum()
        {
            if(!attaque)
            {
                attaque = true;
                yield return new WaitForSeconds(attaqueRate);
                Collider[] colliders = Physics.OverlapSphere(transform.position, this.radiusAttaque);
                foreach (Collider c in colliders)
                {
                     if(c.transform.tag == "Player" || c.transform.tag == "nlPlayer")
                        {
                            c.transform.GetComponent<Player>().ReceiveDamage(this.degatZombie);
                        }
                }
                attaque = false;
            }

        }
    }
}