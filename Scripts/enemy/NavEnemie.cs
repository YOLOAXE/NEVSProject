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
        private int patrolTarget = 0;
        private GameObject target = null;

        [Header("Combat")]
        [SerializeField] private bool inFight = false;
        [SerializeField] private GameObject targetPlayer = null;

        [Header("Raycast")]
        [SerializeField] private GameObject raycastPoint = null;
        [SerializeField] private LayerMask lm = 0;
        private RaycastHit hit;

        #region Start & Stop Callbacks

        void Update()
        {
            if(!isServer){return;}
            patrol();
        }

        private void patrol()
        {
            if (Vector3.Distance(this.transform.position, this.path[this.patrolTarget].transform.position) < 2 && !inFight)
            {
                this.patrolTarget = (++this.patrolTarget) % path.Count;
                agent.SetDestination(path[this.patrolTarget].transform.position);
            }
        }

        [ServerCallback]
        void OnTriggerStay(Collider other)
        {
            if(other.transform.tag == "nlPlayer" || other.transform.tag == "player")
            {
                raycastPoint.transform.LookAt(other.transform);              
                if (this.targetPlayer)
                {
                        
                }
                else
                {

                }
            }
        }

        private bool isOnSight()
        {
            if (Physics.Raycast(this.raycastPoint.transform.position, this.raycastPoint.transform.TransformDirection(Vector3.forward), out this.hit, Mathf.Infinity, lm))
            {
                return (hit.transform.tag == "nlPlayer" || hit.transform.tag == "player");
            }
            return false;
        }
        #endregion
    }
}
