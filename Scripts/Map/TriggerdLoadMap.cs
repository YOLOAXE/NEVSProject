using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerdLoadMap : MonoBehaviour
{
    [SerializeField] private LoadMap lm = null;
    [SerializeField] private bool state = false;
    [SerializeField] private int id = 0;

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            lm.map(state,id);
        }
    }
}
