using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using VHS;

public class turret : NavEnemie
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
    [SerializeField] private GameObject pivot1 = null;
    [SerializeField] private GameObject pivot2 = null;
    private float angleY = 0f;
    private float angleX = 0f;
    private RaycastHit hit;
    private Ray ray;
    Vector3 lookPos = Vector3.zero;
    Quaternion lookAngle = Quaternion.identity;
    [Space, Header("Dead Setting")]
    [SerializeField] private MeshRenderer[] desabledDeath = null;

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
                        seePlayer = false;
                    }
                }
            }
        }
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
            base.m_animator.SetBool("fire", false);
            yield return new WaitForSeconds(shootRate);
            this.seePlayer = false;
            this.shoot = false;
        }
    }

    public override void cannonTargetPlayer()
    {
        if (base.targetPlayer)
        {
            this.shootPoint.transform.LookAt(base.targetPlayer.transform.position + new Vector3(0f, 0.5f, 0f));
            Vector3 relative = transform.InverseTransformPoint(base.targetPlayer.transform.position);
            angleY = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
            angleX = Mathf.Atan2(relative.y, relative.z) * Mathf.Rad2Deg;
            pivot1.transform.localEulerAngles = new Vector3(-90.0f, angleY, 90.0f);
            pivot2.transform.localEulerAngles = new Vector3(0.0f, angleX, 0.0f);
        }
    }

    public override IEnumerator dead()
    {        
        base.ps.Play();
        foreach(MeshRenderer dd in desabledDeath)
        {
            dd.enabled = false;
        }
        yield return new WaitForSeconds(base.tempsMort);
        NetworkServer.Destroy(gameObject);       
    }

    #endregion
}
