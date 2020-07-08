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
        [Header("Raycast Setting")]
        [SerializeField] private int balleparTire = 10;
 
        [SerializeField] private float degatParBalle = 10.0f;
        [SerializeField] private float Z = 10;
        [SerializeField] private float scale = 0.1f;
        private RaycastHit hit;
        private Ray ray;
        Vector3 lookPos = Vector3.zero;
        Quaternion lookAngle = Quaternion.identity;

        #region Start & Stop Callbacks
        public override void attack()
        {
            base.agent.isStopped = true;
            if (Vector3.Distance(this.transform.position, base.targetPlayer.transform.position) > base.huntingDistance)
            {
                base.agent.SetDestination(base.targetPlayer.transform.position);
                base.agent.isStopped = false;              
            }
            else
            {
                if(base.targetPlayer)
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

        [Server]
        IEnumerator ShootIEnum()
        {
            if(!this.shoot)
            {
                this.shoot = true;
                for (byte i = 0; i < this.balleparTire; i++)
                {
                    Vector3 direction = Random.insideUnitCircle * this.scale;
                    direction.z = this.Z;
                    direction = this.shootPoint.transform.TransformDirection(direction.normalized);
                    this.ray = new Ray(this.shootPoint.transform.position, direction);
                    if (Physics.Raycast(this.ray, out hit, Mathf.Infinity, this.lm))
                    {

                    }
                    yield return new WaitForSeconds(bulletRate);
                }
                yield return new WaitForSeconds(shootRate);
                this.shoot = false;
            }
        }
        #endregion
    }
}

