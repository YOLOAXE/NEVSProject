using UnityEngine;
using Mirror;
using System.Collections.Generic;

[System.Serializable]
public class SpawnInfoObject
{
    [SerializeField] private GameObject spawnPoint = null;
    [SerializeField] private GameObject ObjectSpawned = null;

    public void setHasSpawn(GameObject state)
    {
        this.ObjectSpawned = state;
    }
    public GameObject getHasSpawn()
    {
        return this.ObjectSpawned;
    }

    public GameObject getSpawnPoint()
    {
        return this.spawnPoint;
    }
}

public class SpawnWeapon : NetworkBehaviour
{
    [SerializeField] private GameObject spawnObject = null;
    [SerializeField] private int nbObjectSpawn = 0;
    [SerializeField] private SpawnInfoObject[] sIO = null;

    #region Start & Stop Callbacks

    public override void OnStartServer()
    {
        for(int i = 0; i < nbObjectSpawn;i++)
        {
            spawnObjectServer();
        }
    }

    [Server]
    private void spawnObjectServer()
    {
        int id = 0;
        GameObject io = null;
        for (int i = 0; i < sIO.Length; i++)
        {
            if (sIO[i].getHasSpawn() == null)
            {
                for (int j = 0; j < nbObjectSpawn; j++)
                {
                    id = Random.Range(0, sIO.Length);
                    if (sIO[id].getHasSpawn() == null)
                    {
                        io = Instantiate(this.spawnObject, sIO[id].getSpawnPoint().transform.position, sIO[id].getSpawnPoint().transform.rotation);
                        sIO[id].setHasSpawn(io);
                        if(io.GetComponent<ItemAbstract>())
                        {
                            io.GetComponent<ItemAbstract>().SetScript(this);
                        }
                        NetworkServer.Spawn(io);
                        return;
                    }
                }
                io = Instantiate(this.spawnObject, sIO[i].getSpawnPoint().transform.position, sIO[i].getSpawnPoint().transform.rotation);
                sIO[i].setHasSpawn(io);
                NetworkServer.Spawn(io);
                return;
            }
        } 
    }
    [Server]
    private int testObject()
    {
        int nbO = 0;
        foreach (SpawnInfoObject s in sIO)
        {
            if(s.getHasSpawn())
            {
                nbO++;
            }
        }
        return nbObjectSpawn-nbO;
    }

    [Server]
    public void getObjectByPlayer(GameObject objectGPP)
    {
        if (!isServer) { return; }
        spawnObjectServer();
        foreach(SpawnInfoObject s in sIO)
        {
            if(objectGPP == s.getHasSpawn())
            {
                s.setHasSpawn(null);
            }
        }
        int tResult = testObject();
        for (int i = 0; i < tResult; i++)
        {
            spawnObjectServer();
        }
    }
    #endregion
}
