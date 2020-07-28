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
        [SerializeField] private float vitesseBase = 1f;
        [SerializeField] private float vitesseSprint = 2f;
        private GameObject target = null;

        [Header("Detection")]
        [SerializeField] protected EnemieDetection ed = null;

        [Header("Combat")]
        [SerializeField] private bool inFight = false;
        [SerializeField] [SyncVar] protected GameObject targetPlayer = null;
        [SerializeField] private float timeFight = 15f;
        [SerializeField] protected float tempsMort = 3f;
        private float timerF = -20f;

        [Header("Mort")]
        [SerializeField] protected ParticleSystem ps;
        [SyncVar] protected bool isDead = false;

        [Header("Animation")]
        [SerializeField] protected Animator m_animator = null;

        void Start()
        {
            StartCoroutine(CSound());
        }
        
        #region Start & Stop Callbacks

        public override void OnStartServer()
        {
            agent.SetDestination(path[this.patrolTarget].transform.position);
        }

        void Update()
        {
            cannonTargetPlayer();
            if (!isServer || this.isDead) {return;}
            patrol();
            TargetPlayerTime();
            if(this.inFight)
            {
                this.agent.speed = this.vitesseSprint;
                this.attack();
            }
            m_animator.SetFloat("Speed", this.agent.desiredVelocity.magnitude);
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
                    this.agent.speed = this.vitesseBase;
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
            if (TPlayer != this.targetPlayer)
            {
                this.targetPlayer = TPlayer;
            }
        }

        public void ReceiveDamage(float damage,GameObject player)
        {
            if (isServer)
            {
                this.currentHealth -= damage;
                addTargetPlayer(player);
                if (this.currentHealth <= 0 && !this.isDead)
                {
                    this.isDead = true;
                    agent.isStopped = true;
                    m_animator.SetBool("dead", true);
                    RpcdeadStartC();
                }
            }
        }
        [ClientRpc]
        void RpcdeadStartC()
        {
            StartCoroutine(dead());
        }

        public virtual IEnumerator dead()
        {
            ps.transform.eulerAngles = new Vector3(0.0f, 150.0f, 0.0f);
            yield return new WaitForSeconds(3f);            
            ps.Play();
            yield return new WaitForSeconds(tempsMort);
            NetworkServer.Destroy(gameObject);
        }

        public virtual void attack() { }
        public virtual void cannonTargetPlayer() { }

        #endregion
        public virtual IEnumerator CSound() { yield return null; }
    }
}
