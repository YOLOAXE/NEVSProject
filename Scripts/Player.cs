using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float noise = 0.1f;
    [SerializeField] private Animator anim = null;
    [SerializeField] private CharacterController controller = null;

    void Update()
    {
        anim.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        anim.SetFloat("Vertical", Input.GetAxis("Vertical") * (Input.GetButton("Sprint") ? 2f : 1f));
        noise = ((Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) * (Input.GetButtonDown("Sprint") ? 1f : 2f)) + 0.1f;
        anim.SetBool("jump", controller.isGrounded);
    }

    public float getNoise()
    {
        return this.noise;
    }
}
