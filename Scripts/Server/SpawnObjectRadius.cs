using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;

public class SpawnObjectRadius : NetworkBehaviour
{
    [SerializeField] private GameObject spawnObject = null;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private float spawnRadius = 20f;

    #region Start & Stop Callbacks
    public override void OnStartServer()
    {
        StartCoroutine(SpawnObject());
    }

    #endregion

    private IEnumerator SpawnObject()
    {
        while (true)
        {
            GameObject io = Instantiate(spawnObject,new Vector3(Random.Range(-spawnRadius, spawnRadius) + transform.position.x, transform.position.y, Random.Range(-spawnRadius, spawnRadius) + transform.position.z),Quaternion.identity);
            NetworkServer.Spawn(io);
            yield return new WaitForSeconds(spawnRate);
        }
    }
}
