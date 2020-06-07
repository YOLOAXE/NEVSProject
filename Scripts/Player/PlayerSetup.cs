using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private GameObject[] objectDisable = null;
    [SerializeField] private Behaviour[] scriptDisable = null;
    [SerializeField] private string tagNamePlayer = "Player";
    [SerializeField] private string tagNotLocalPlayer = "nlPlayer";
    [SerializeField] private GameObject[] dfObjectChange = null;
    [SerializeField] private GameObject camRotate = null;
    [SerializeField] private GameObject rigSpine = null;

    void Start()
    {
        if (!isLocalPlayer)
        {
            for (byte i = 0; i < objectDisable.Length; i++)
            {
                objectDisable[i].SetActive(false);
            }
            for (byte i = 0; i < scriptDisable.Length; i++)
            {
                scriptDisable[i].enabled = false;
            }
            transform.tag = tagNotLocalPlayer;
            camRotate.transform.parent = rigSpine.transform;
        }
        else
        {
            transform.tag = tagNamePlayer;
            foreach(GameObject o in dfObjectChange)
            {
                o.layer = LayerMask.NameToLayer("DrawAlways");
            }
        }
    }
}
