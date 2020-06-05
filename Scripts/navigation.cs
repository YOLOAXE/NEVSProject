using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navigation : MonoBehaviour
{

    [SerializeField] private GameObject[] path = new GameObject[4];
    [SerializeField] private NavMeshAgent agent = null;
    private int target = 0;
    // Start is called before the first frame update
    void Start()
    {
        agent.SetDestination(path[this.target].transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(this.transform.position, this.path[this.target].transform.position) < 3) {
            this.target = (++this.target) % path.Length;
            agent.SetDestination(path[this.target].transform.position);
        }
    }
}
