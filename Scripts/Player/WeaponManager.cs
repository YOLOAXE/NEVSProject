using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DitzelGames.FastIK
{
    [System.Serializable]
    public class Hand
    {
        [SerializeField] private GameObject targetHand = null;
        [SerializeField] private GameObject rigHand = null;

        public GameObject getTH()
        {
            return this.targetHand;
        }
        public GameObject getRH()
        {
            return this.rigHand;
        }
        public void AplyAnimation(bool state)
        {
            this.rigHand.GetComponent<FastIKFabric>().enabled = !state;
            if(state)
            {
                this.targetHand.transform.position = this.rigHand.transform.position;
            }
        }
        public void AplyArmePosAngle(Vector3 pos, Vector3 angle)
        {
            this.targetHand.transform.localPosition = pos;
            this.targetHand.transform.localEulerAngles = angle;
        }
    }
    [System.Serializable]
    public class WeaponInfo
    {
        [SerializeField] private string Name = "";
        [SerializeField] private int idArme = 0;
        [SerializeField] private Vector3[] handPos = null;
        [SerializeField] private Vector3[] handAngle = null;
        [SerializeField] private bool[] freeHand = null;
        [SerializeField] private bool[] AnimationHand = null;

        public bool GetFreeHand(byte indice)
        {
            return this.freeHand[indice];
        }

        public Vector3 GetTabHandPos(byte indice)
        {
            return this.handPos[indice];
        }

        public Vector3 GetTabHandAngle(byte indice)
        {
            return this.handAngle[indice];
        }
    }

    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private Hand[] tHand = null;
        [SerializeField] private WeaponInfo[] wI = null;
        [SerializeField] private int currentIDArme = 0;
        [SerializeField] private GameObject pivotCamera = null;
        [SerializeField] private GameObject turnHandObject = null;

        void LateUpdate()
        {
            for (byte i = 0; i < tHand.Length; i++)
            {
                tHand[i].AplyAnimation(wI[currentIDArme].GetFreeHand(i));
                if (!wI[currentIDArme].GetFreeHand(i))
                {
                    turnHandObject.transform.localEulerAngles = pivotCamera.transform.localEulerAngles;
                    tHand[i].AplyArmePosAngle(wI[currentIDArme].GetTabHandPos(i), wI[currentIDArme].GetTabHandAngle(i));
                }
            }
            if(Input.GetKeyDown(KeyCode.F))
            {
                currentIDArme = currentIDArme == 0 ? 1 : 0;
            }
        }
    }
}