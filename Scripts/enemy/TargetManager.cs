using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TargetManager : NetworkBehaviour
{
    
    void Start()
    {
        if(isServer)
        {
            StartCoroutine(targetsUpdate());
        }
    }

    // Update is called once per frame
    IEnumerator targetsUpdate()
    {
        while (true)
        {
            //du bordel la dedans
            yield return new WaitForSeconds(10f);
        } 
    }
}
