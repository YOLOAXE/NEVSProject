using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace VHS
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private float noise = 0.1f;
        [SerializeField] private Animator anim = null;
        [SerializeField] private CharacterController controller = null;
        
        void Update()
        {
            if(!isLocalPlayer)
            {
                return;
            }
            AplyAnimation();
        }

        public void AplyAnimation()
        {
            anim.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
            anim.SetFloat("Vertical", Input.GetAxis("Vertical") * (Input.GetButton("Sprint") ? 2f : 1f));
            noise = ((Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) * (Input.GetButtonDown("Sprint") ? 1f : 2f)) + 0.1f;
            //le noise est a revoir
            anim.SetBool("jump", controller.isGrounded && !Input.GetButton("Jump"));
        }

        public float getNoise()
        {
            return this.noise;
        }
    }
}