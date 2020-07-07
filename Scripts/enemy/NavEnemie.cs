using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

namespace VHS
{
    public abstract class NavEnemie : NetworkBehaviour
    {
        [Header("EnemieInfo")]
        [SerializeField] protected NavMeshAgent agent = null;
        [SerializeField] private float currentHealth = 100.0f;
        [SerializeField] private float maxHealth = 100.0f;

        [Header("Deplacement")]       
        [SerializeField] private List<GameObject> path = new List<GameObject>();
        [SerializeField] protected float huntingDistance = 10f;
        [SerializeField] protected float attaqueDistance = 15f;
        [SerializeField] private float pathChangeDistance = 2f;
        [SerializeField] protected int patrolTarget = 0;
        [SerializeField] private bool research = false;
        [SerializeField] private int researchNb = 3;
        private GameObject target = null;

        [Header("Detection")]
        [SerializeField] private EnemieDetection ed = null;

        [Header("Combat")]
        [SerializeField] private bool inFight = false;
        [SerializeField] protected GameObject targetPlayer = null;
        [SerializeField] private float timeFight = 15f;
        [SerializeField] private float tempsMort = 3f;
        private float timerF = -20f;

        [Header("Mort")]
        [SerializeField] private AnimationCurve fadeIn;
        [SerializeField] private Renderer[] _renderer;
        [SerializeField] private ParticleSystem ps;
        private bool isDead = false;

        #region Start & Stop Callbacks

        public override void OnStartServer()
        {
            path.Add(GameObject.Find("PathPoint (1)"));
            path.Add(GameObject.Find("PathPoint (2)"));
            path.Add(GameObject.Find("PathPoint (3)"));
            path.Add(GameObject.Find("PathPoint (4)"));
            agent.SetDestination(path[this.patrolTarget].transform.position);
        }

        void Update()
        {
            if(!isServer){return;}
            patrol();
            TargetPlayerTime();
            if(this.inFight)
            {
                this.attack();
            }
            
        }

        private void patrol()
        {
            if (Vector3.Distance(this.transform.position, this.path[this.patrolTarget].transform.position) < pathChangeDistance && !inFight && !this.research)
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
                this.research = true;
                this.ed.ResetTP();
                this.targetPlayer = null;
                agent.isStopped = false;
                agent.SetDestination(path[this.patrolTarget].transform.position);
                
            } 
            
            if (this.research && this.agent.desiredVelocity.magnitude < 0.1f)
            {
                agent.SetDestination(new Vector3(this.transform.position.x + Random.Range(-huntingDistance, huntingDistance),
                                                 this.transform.position.y,
                                                 this.transform.position.z + Random.Range(-huntingDistance, huntingDistance)));
                this.researchNb--;
                if (this.researchNb == 0)
                {
                    this.research = false;
                    this.researchNb = 3;
                    agent.SetDestination(path[this.patrolTarget].transform.position);
                }
            }
        }

        public void addTargetPlayer(GameObject TPlayer)
        {
            this.inFight = true;
            this.timerF = this.timeFight;
            this.research = false;
            this.researchNb = 3;
            this.targetPlayer = TPlayer;
        }

        public void ReceiveDamage(float damage,GameObject player)
        {
            this.currentHealth -= damage;
            addTargetPlayer(player);
            if(this.currentHealth <= 0 && !this.isDead)
            {
                this.isDead = true;
                RpcdeadStartC();
            }
        }
        [ClientRpc]
        void RpcdeadStartC()
        {
            StartCoroutine(dead());
        }

        private IEnumerator dead()
        {
            float timer = 0;
            int shaderProperty = Shader.PropertyToID("_cutoff");
            var main = ps.main;
            main.duration = tempsMort/2;
            ps.Play();

            while (timer < tempsMort/2)
            {
                foreach (Renderer r in _renderer)
                {
                    r.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, tempsMort/2, timer)));
                }
                timer += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(tempsMort);
            NetworkServer.Destroy(gameObject);
        }

        public virtual void attack() { }
        
        #endregion
    }
}
