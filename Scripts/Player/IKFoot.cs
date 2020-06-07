using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFoot : MonoBehaviour
{
    [SerializeField] private GameObject rigTarget = null;
    [SerializeField] private LayerMask lm = 0;
    RaycastHit hit;

    void LateUpdate()
    {
        transform.position = rigTarget.transform.position;
        if (Physics.Raycast(transform.position + new Vector3(0.0f,0.5f,0.0f), -transform.parent.TransformDirection(Vector3.up), out hit, Mathf.Infinity,lm))
        {
            Vector3 aG = Quaternion.FromToRotation(Vector3.forward, hit.normal).ToEulerAngles();
            transform.localEulerAngles = new Vector3(aG.x, -180.0f, 0);

        }
    }
}
