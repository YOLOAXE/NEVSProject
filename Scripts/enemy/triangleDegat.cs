using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.MultipleAdditiveScenes;

public class triangleDegat : NetworkBehaviour
{
    [SerializeField] private List<GameObject> player = new List<GameObject>();
    [SerializeField] private NetworkAnimator na = null;
    [SerializeField] private float degatPlayer = 5f;
    [SerializeField] private float degatRate = 1f;
    [SerializeField] private float desactiveAfter = 33f;
    [SerializeField] private float radiusTp = 12;
    private float timerDegat = 0f;
    [Header("Deplacement")]
    [SerializeField] private float pathChange = 1.5f;
    [SerializeField] private float movementSpeed = 20;
    [SerializeField] private float minWaitAD = 2f;
    [SerializeField] private float maxWaitAD = 10f;
    [SerializeField] private AnimationCurve curveDeplacement;
    private Vector3 departPosition = new Vector3(0, 0, 0);
    private Vector3 originalPosition = new Vector3(0, 0, 0);
    private Vector3 posDestination = new Vector3(0, 0, 0);
    private float timerD = 0f;
    private bool canMove = false;
    private bool isDesactive = false;

    void Start()
    {
        if (!isServer) { return; }
        Invoke(nameof(desactiveTrig), desactiveAfter);
        Invoke(nameof(Destroy), (desactiveAfter + 2));
        player = GameObject.Find("NetworkManager").GetComponent<GameNetworkManager>().GetPlayerList();
        originalPosition = transform.position;
        timerDegat = degatRate;
        initDeplacementValue();
        StartCoroutine(waitDeplacement());
    }

    void Update()
    {
        if (timerDegat <= 0 && !isDesactive)
        {
            timerDegat = degatRate;
            foreach (GameObject p in player)
            {
                if (p)
                {
                    p.GetComponent<Player>().ReceiveDamage(degatPlayer);
                }
            }
        }
        else
        {
            timerDegat -= Time.deltaTime;
        }
        float dist = Vector3.Distance(posDestination, transform.position);
        if (dist < pathChange)
        {
            timerD -= Time.deltaTime;
            if (timerD < 0)
            {
                initDeplacementValue();
            }
        }
        else
        {
            if (canMove)
            {
                transform.position = Vector3.MoveTowards(transform.position, posDestination, movementSpeed * Time.deltaTime * curveDeplacement.Evaluate(dist / Vector3.Distance(departPosition, posDestination)));
            }
        }
    }

    IEnumerator waitDeplacement()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    void initDeplacementValue()
    {
        posDestination = originalPosition + Random.insideUnitSphere * radiusTp;
        posDestination.y = originalPosition.y;
        timerD = Random.Range(minWaitAD, maxWaitAD);
        departPosition = transform.position;
    }

    [Server]
    public void desactiveTrig()
    {
        na.SetTrigger("Active");
        isDesactive = true;
    }

    [Server]
    public void Destroy()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "nlPlayer" || other.transform.tag == "Player")
        {
            player.Remove(other.gameObject);
        }
    }

    [ServerCallback]
    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "nlPlayer" || other.transform.tag == "Player")
        {
            player.Add(other.gameObject);
        }
    }

    [Server]
    public void stop()
    {
        Destroy();
    }
}


