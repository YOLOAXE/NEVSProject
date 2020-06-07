using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VHS
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float noise = 0.1f;
        [SerializeField] private Animator anim = null;
        [SerializeField] private CharacterController controller = null;
        [SerializeField] private float timeAplyIsGrounded = 0.1f;
        private float varTAIG = 0;
        
        void Update()
        {
            AplyAnimation();
        }

        public void AplyAnimation()
        {
            anim.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
            anim.SetFloat("Vertical", Input.GetAxis("Vertical") * (Input.GetButton("Sprint") ? 2f : 1f));
            noise = ((Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) * (Input.GetButtonDown("Sprint") ? 1f : 2f)) + 0.1f;
            if (!controller.isGrounded)
            { varTAIG -= Time.deltaTime; }
            else
            { varTAIG = timeAplyIsGrounded; }
            anim.SetBool("jump", varTAIG >= 0);
        }

        public float getNoise()
        {
            return this.noise;
        }
    }
}