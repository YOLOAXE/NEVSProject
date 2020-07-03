using UnityEngine;
using Mirror;
using System.Collections.Generic;

namespace VHS
{
    public class EnemieDetection : NetworkBehaviour
    {
        [Header("Raycast")]
        [SerializeField] private GameObject raycastPoint = null;
        [SerializeField] private LayerMask lm = 0;
        private RaycastHit hit;
        [Header("NavEnemie")]
        [SerializeField] private NavEnemie ne = null;
        [SerializeField] private GameObject targetPlayer = null;

        #region Start & Stop Callbacks

        [ServerCallback]
        void OnTriggerStay(Collider other)
        {
            if (other.transform.tag == "nlPlayer" || other.transform.tag == "Player")
            {
                raycastPoint.transform.LookAt(other.transform.position + new Vector3(0f,0.5f,0f));
                if (Physics.Raycast(this.raycastPoint.transform.position, this.raycastPoint.transform.TransformDirection(Vector3.forward), out this.hit, Mathf.Infinity, lm))
                {
                    Debug.DrawRay(this.raycastPoint.transform.position, this.raycastPoint.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
                    if (other.transform.gameObject == this.hit.transform.gameObject)
                    {
                        if(this.targetPlayer)
                        {
                            if(Vector3.Distance(transform.position, this.targetPlayer.transform.position) > Vector3.Distance(transform.position, this.hit.transform.position))
                            {
                                this.targetPlayer = this.hit.transform.gameObject;
                                this.ne.addTargetPlayer(this.targetPlayer);
                            }
                        }
                        else
                        {
                            this.targetPlayer = this.hit.transform.gameObject;
                            this.ne.addTargetPlayer(this.targetPlayer);
                        }
                    }
                }
            }
        }

        public void ResetTP()
        {
            this.targetPlayer = null;
        }
        #endregion
    }
}
