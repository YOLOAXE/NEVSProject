using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

namespace VHS
{
    public class Boss : NetworkBehaviour
    {
        [Header("Boss Setting")]
        [SerializeField] protected NavMeshAgent agent = null;
        [SerializeField] private float currentHealth = 100.0f;
        [SerializeField] private float maxHealth = 100.0f;
        [Header("Deplacement")]
        [SerializeField] private List<GameObject> AllPlayer = null;

        [Server]
        public virtual void ReceiveAllPlayer(List<GameObject> al)
        {
            this.AllPlayer.Clear();
            this.AllPlayer = al;
        }
    }
}