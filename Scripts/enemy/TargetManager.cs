using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using VHS;

public class TargetManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] players = null;
    [SerializeField] private GameObject[] navE = null;
    private GameObject[] pl;
    private GameObject[] plnpl;

    void Start()
    {
        if(isServer || true)
        {
            StartCoroutine(targetsUpdate());
        }
    }

    IEnumerator targetsUpdate()
    {
        while (true)
        {
            this.navE = GameObject.FindGameObjectsWithTag("Ennemie");
            this.pl = GameObject.FindGameObjectsWithTag("Player");
            this.plnpl = GameObject.FindGameObjectsWithTag("nlPlayer");
            players = new GameObject[pl.Length + plnpl.Length];
            this.pl.CopyTo(players, 0);
            this.plnpl.CopyTo(players, pl.Length);
            foreach(GameObject ne in navE)
            {
                ne.GetComponent<NavEnemie>().setPLayers(this.players);
            }
            yield return new WaitForSeconds(10f);
        } 
    }
}
