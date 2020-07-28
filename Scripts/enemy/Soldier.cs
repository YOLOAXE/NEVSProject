using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

namespace VHS
{
    public class Soldier : NavEnemie
    {
        [Header("Attaque Setting")]
        [SerializeField] private GameObject shootPoint = null;
        [SerializeField] private LayerMask lm = 0;
        [SerializeField] private float shootRate = 0.5f;
        [SerializeField] private float bulletRate = 0.1f;
        [SerializeField] private bool shoot = false;
        [SerializeField] private bool attaque = false;
        private float distance = 0f;
        [Header("Raycast Setting")]
        [SerializeField] private int balleparTire = 10;
        [SerializeField] private float degatParBalle = 10.0f;
        [SerializeField] private float Z = 10;
        [SerializeField] private float scale = 0.1f;
        [SerializeField] private bool seePlayer = false;
        private RaycastHit hit;
        private Ray ray;
        Vector3 lookPos = Vector3.zero;
        Quaternion lookAngle = Quaternion.identity;

        #region Start & Stop Callbacks
        public override void attack()
        {
            base.agent.isStopped = true;
            if (base.targetPlayer == null) { return; }

            this.distance = Vector3.Distance(this.transform.position, base.targetPlayer.transform.position);
            if (base.targetPlayer.GetComponent<Player>().GetIsAlive())
            {
                if ((distance > base.huntingDistance && !attaque) || (distance > (base.huntingDistance * 1.5f) && attaque))
                {
                    if (!seePlayer)
                    {
                        base.agent.SetDestination(base.targetPlayer.transform.position);
                        base.agent.isStopped = false;
                        attaque = false;
                    }
                }
                else
                {
                    Vector3 direction = Random.insideUnitCircle * this.scale;
                    direction.z = 10;
                    direction = this.shootPoint.transform.TransformDirection(direction.normalized);
                    this.ray = new Ray(this.shootPoint.transform.position, direction);
                    if (Physics.Raycast(this.ray, out hit, Mathf.Infinity, this.lm))
                    {
                        if (hit.transform.tag == "Player" || hit.transform.tag == "nlPlayer")
                        {
                            attaque = true;
                            seePlayer = true;
                            StartCoroutine(ShootIEnum());
                        }
                        else
                        {
                            base.agent.SetDestination(base.targetPlayer.transform.position);
                            base.agent.isStopped = false;
                            seePlayer = false;
                        }
                    }
                }
            }
            else
            {
                base.targetPlayer = null;
                base.ed.ResetTP();
                return;
            }
            lookPos = this.targetPlayer.transform.position - this.transform.position;
            lookPos.y = 0;
            lookAngle = Quaternion.LookRotation(lookPos);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, lookAngle, Time.deltaTime * (base.agent.angularSpeed / 30));
        }

        [Server]
        IEnumerator ShootIEnum()
        {
            if (!this.shoot)
            {
                this.shoot = true;
                base.m_animator.SetBool("fire", true);
                for (byte i = 0; i < this.balleparTire; i++)
                {
                    Vector3 direction = Random.insideUnitCircle * this.scale;
                    direction.z = this.Z;
                    direction = this.shootPoint.transform.TransformDirection(direction.normalized);
                    this.ray = new Ray(this.shootPoint.transform.position, direction);
                    if (Physics.Raycast(this.ray, out hit, Mathf.Infinity, this.lm))
                    {
                        if (hit.transform.tag == "Player" || hit.transform.tag == "nlPlayer")
                        {
                            hit.transform.GetComponent<Player>().ReceiveDamage(degatParBalle);
                        }
                    }
                    yield return new WaitForSeconds(bulletRate);
                }
                base.m_animator.SetBool("aim", true);
                base.m_animator.SetBool("fire", false);
                yield return new WaitForSeconds(shootRate);
                base.m_animator.SetBool("aim", false);
                this.seePlayer = false;
                this.shoot = false;
            }
        }

        public override void cannonTargetPlayer()
        {
            if (base.targetPlayer)
            {
                this.shootPoint.transform.LookAt(base.targetPlayer.transform.position + new Vector3(0f, 0.5f, 0f));
            }
        }
        #endregion
    }
}

