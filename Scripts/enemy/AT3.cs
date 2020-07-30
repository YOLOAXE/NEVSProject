using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AT3 : AttaqueBoss
{
    [Header("Deplacement")]
    [SerializeField] private GameObject[] pointDeplacement = null;
    [SerializeField] private GameObject pointTargetView = null;
    [SerializeField] private int indiceDeplacement = 0;
    [SerializeField] private float pathChange = 1.5f;
    [SerializeField] private float movementSpeed = 20;
    [SerializeField] private float minWaitAD = 2f;
    [SerializeField] private float maxWaitAD = 10f;
    [SerializeField] private AnimationCurve curveDeplacement;
    private Vector3 departPosition = new Vector3(0, 0, 0);
    private float timerD = 0f;
    private float angleY = 0f;
    [Header("Attaque Setting")]
    [SerializeField] private GameObject triangleDegat = null;
    [SerializeField] private float radiusSpawn = 12f;
    [SerializeField] private float tempsSpawn = 1.5f;
    [SerializeField] private float tempsAttaque = 35f;
    [SerializeField] private List<GameObject> listTrigD = new List<GameObject>();
    private float timerA = 0;
    private float timerS = 0;
    [Header("Animator")]
    [SerializeField] private Animator anim = null;
    [SerializeField] private bool intro = false;
    [SerializeField] private float tempsIntro = 2f;
    [Header("ShakeCam")]
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private float magnitude = 0.3f;

    public override void attaqueStart()
    {
        StartCoroutine(AplyIntro());
    }

    public override void attaqueUpdate()
    {
        if (intro) { return; }
        if (timerS <= 0)
        {
            GameObject io = Instantiate(triangleDegat, transform.position + Random.insideUnitSphere * radiusSpawn, Quaternion.identity);
            io.transform.position = new Vector3(io.transform.position.x, transform.position.y, io.transform.position.z);
            listTrigD.Add(io);
            NetworkServer.Spawn(io);
            timerS = tempsSpawn;
        }
        if (timerA <= 0)
        {
            anim.SetBool("Open", false);
            base.b.EndAttaque();
        }
        timerA -= Time.deltaTime;
        timerS -= Time.deltaTime;
    }

    public override void deplacement()
    {
        if (intro) { return; }
        float dist = Vector3.Distance(pointDeplacement[indiceDeplacement].transform.position, base.b.transform.position);
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
            base.b.transform.position = Vector3.MoveTowards(base.b.transform.position, this.pointDeplacement[indiceDeplacement].transform.position, movementSpeed * Time.deltaTime * curveDeplacement.Evaluate(dist / Vector3.Distance(departPosition, this.pointDeplacement[indiceDeplacement].transform.position)));
        }
        Vector3 relative = this.pointTargetView.transform.InverseTransformPoint(base.b.transform.position);
        angleY = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        base.b.transform.eulerAngles = new Vector3(0, angleY, 0);
    }

    void initDeplacementValue()
    {
        indiceDeplacement = Random.Range(0, pointDeplacement.Length);
        timerD = Random.Range(minWaitAD, maxWaitAD);
        departPosition = base.b.transform.position;
        base.b.AplyTriggerNe("Dash");
    }

    void InitTemps()
    {
        timerS = 0;
        timerA = tempsAttaque;
    }

    IEnumerator AplyIntro()
    {
        intro = true;
        base.b.AplyTriggerNe("attaqueIntro");
        base.b.RpcShakeAllPlayer(duration, magnitude);
        listTrigD.Clear();
        yield return new WaitForSeconds(tempsIntro);
        InitTemps();
        initDeplacementValue();
        anim.SetBool("Open", true);
        intro = false;
    }

    public override void resetAt()
    {
        anim.SetBool("Open", false);
        foreach (GameObject o in listTrigD)
        {
            if (o)
            {
                o.GetComponent<triangleDegat>().stop();
            }
        }
    }
}
