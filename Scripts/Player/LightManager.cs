using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private GameObject[] AllLight = null;
    [SerializeField] private float distanceLight = 30.0f;

    void Start()
    {
        AllLight = GameObject.FindGameObjectsWithTag("LightControl");
        StartCoroutine(lightControl());
    }

    IEnumerator lightControl()
    {
        while (true)
        {
            foreach (GameObject o in AllLight)
            {
                o.SetActive(Vector3.Distance(transform.position, o.transform.position) < distanceLight);
            }
            yield return new WaitForSeconds(2.0f);
        }
    }
}
