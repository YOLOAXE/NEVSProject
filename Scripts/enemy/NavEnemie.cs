using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

namespace VHS
{
    public class NavEnemie : NetworkBehaviour
    {
        [Header("EnemieInfo")]
        [SerializeField] private NavMeshAgent agent = null;
        [SyncVar] public float currentHealth = 100.0f;
        [SerializeField] private float maxHealth = 100.0f;

        [Header("Deplacement")]       
        [SerializeField] private List<GameObject> path = new List<GameObject>();
        [SerializeField] private float chaseDistance = 30f;
        [SerializeField] private float attaqueDistance = 15f;
        [SerializeField] private float pathChangeDistance = 2f;
        [SerializeField] private int patrolTarget = 0;
        private GameObject target = null;

        [Header("Detection")]
        [SerializeField] private EnemieDetection ed = null;

        [Header("Combat")]
        [SerializeField] private bool inFight = false;
        [SerializeField] private GameObject targetPlayer = null;
        [SerializeField] private float timeFight = 15f;
        private float timerF = 0f;

        #region Start & Stop Callbacks

        public override void OnStartServer()
        {
            agent.SetDestination(path[this.patrolTarget].transform.position);
        }

        void Update()
        {
            if(!isServer){return;}
            patrol();
            TargetPlayerTime();
        }

        private void patrol()
        {
            if (Vector3.Distance(this.transform.position, this.path[this.patrolTarget].transform.position) < pathChangeDistance && !inFight)
            {
                this.patrolTarget = ((this.patrolTarget+1)%path.Count);
                agent.SetDestination(path[this.patrolTarget].transform.position);
            }
        }

        private void TargetPlayerTime()
        {
            if (this.timerF > 0)
            {
                this.timerF -= Time.deltaTime;
            }
            else if (this.timerF > -5)
            {
                this.timerF = -20;
                this.inFight = false;
                this.ed.ResetTP();
                this.targetPlayer = null;
                agent.isStopped = false;
                agent.SetDestination(path[this.patrolTarget].transform.position);
            }
        }

        public void addTargetPlayer(GameObject TPlayer)
        {
            this.inFight = true;
            agent.isStopped = true;
            this.timerF = this.timeFight;
            this.targetPlayer = TPlayer;
        }
        
        #endregion
    }
}
