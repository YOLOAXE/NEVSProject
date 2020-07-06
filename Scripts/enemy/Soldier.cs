using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

namespace VHS
{
    public class Soldier : NavEnemie
    {
        Vector3 lookPos = Vector3.zero;
        Quaternion lookAngle = Quaternion.identity;
        #region Start & Stop Callbacks
        public override void attack()
        {
            if (Vector3.Distance(this.transform.position, base.targetPlayer.transform.position) > base.huntingDistance)
            {
                base.agent.isStopped = false;
                base.agent.SetDestination(new Vector3(Random.Range(-base.huntingDistance, base.huntingDistance), 0, Random.Range(-base.huntingDistance, base.huntingDistance)));
            }
            base.agent.SetDestination(base.targetPlayer.transform.position);

            lookPos = this.targetPlayer.transform.position - this.transform.position;
            lookPos.y = 0;
            lookAngle = Quaternion.LookRotation(lookPos);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, lookAngle, Time.deltaTime * (base.agent.angularSpeed/10));
        }

        #endregion
    }
}

