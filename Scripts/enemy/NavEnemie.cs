using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

namespace VHS
{
    public class NavEnemie : NetworkBehaviour
    {

        [SerializeField] private GameObject[] path = new GameObject[4];
        [SerializeField] private GameObject[] players = null;
        [SerializeField] private NavMeshAgent agent = null;
        private int patrolTarget = 0;
        private GameObject target = null;
        [SerializeField] private float targetingRange = 10f;
        [SerializeField] private LayerMask lm = 0;

        void Start()
        {
            if (!isServer) { return; }
            agent.SetDestination(path[this.patrolTarget].transform.position);
            StartCoroutine(viewUpdate());
        }


        void Update()
        {
            if(!isServer){return;}
            //code IA
            
        }

        private void patrol()
        {
            if (Vector3.Distance(this.transform.position, this.path[this.patrolTarget].transform.position) < 2)
            {
                this.patrolTarget = (++this.patrolTarget) % path.Length;
                agent.SetDestination(path[this.patrolTarget].transform.position);
            }
        }

        IEnumerator viewUpdate()
        {
            while (true)
            {
                this.target = this.proximityCheck();
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void setPLayers(GameObject[] allPlayer)
        {
            this.players = allPlayer;
        }

        private GameObject proximityCheck()
        {
            if (this.players.Length > 0)
            {
                float actualRange = Vector3.Distance(this.transform.position, this.players[0].transform.position) / this.players[0].GetComponent<Player>().getNoise();
                float range = 0;
                GameObject potentialTarget = this.players[0];

                foreach (GameObject player in players)
                {
                    range = Vector3.Distance(this.transform.position, player.transform.position) / player.GetComponent<Player>().getNoise();
                    if (actualRange > range)
                    {

                        actualRange = range;
                        potentialTarget = player;
                    }
                }
                Debug.Log(Vector3.Distance(this.transform.position, potentialTarget.transform.position));

                if (actualRange <= this.targetingRange)
                {
                    return potentialTarget;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        private bool asTarget()
        {
            if (this.target == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool isOnSight()
        {
            RaycastHit hit;
            Vector3 fromPosition = this.transform.position;
            Vector3 toPosition = this.target.transform.position;
            Vector3 direction = toPosition - fromPosition;


            if (Physics.Raycast(fromPosition, direction, out hit, Mathf.Infinity, lm))
            {
                Debug.DrawLine(transform.position, hit.point, Color.yellow);
                if (hit.collider.gameObject == this.target)
                {
                    return true;
                }
            }
            return false;
        }

        private void find()
        {
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }
    }
}
