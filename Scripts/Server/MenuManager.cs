using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mirror
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private NetworkManager manager;
        [Header("UI Setting")]
        [SerializeField] private GameObject ui = null;
        [SerializeField] private Slider sensibilite = null;
        [SerializeField] private TextMeshProUGUI textPS = null;
        [SerializeField] private TMP_InputField pseudo = null;
        [SerializeField] private TMP_InputField ip = null;
        [SerializeField] private GameObject uiMenuInGame = null;

        void Awake()
        {
            GetSave();
        }

        public void LanClient()
        {
            ui.SetActive(false);
            manager.StartClient();
            StartCoroutine(VerificationConnection());
        }

        void GetSave()
        {
            ip.text = PlayerPrefs.GetString("ipAdress", "LocalHost");
            pseudo.text = PlayerPrefs.GetString("Pseudo", "Joueur");
            sensibilite.value = PlayerPrefs.GetFloat("Sensibilite",150);
        }

        public void Save()
        {
            manager.networkAddress = ip.text;
            PlayerPrefs.SetString("ipAdress", ip.text);
            PlayerPrefs.SetString("Pseudo", pseudo.text);
            PlayerPrefs.SetFloat("Sensibilite", sensibilite.value);
        }

        public void AplyButtonMenu()
        {
            uiMenuInGame.transform.localPosition = new Vector3(0, 10000, 0);
        }

        public void Update()
        {
            textPS.text = System.Math.Round((sensibilite.value / sensibilite.maxValue), 2).ToString();
        }

        public void LanHost()
        {
            if (!NetworkClient.isConnected && !NetworkServer.active)
            {
                if (!NetworkClient.active)
                {
                    // LAN Host
                    if (Application.platform != RuntimePlatform.WebGLPlayer)
                    {
                        ui.SetActive(false);
                        manager.StartHost();
                    }
                }
            }
        }

        public void Exit()
        {
            StopClientHost();
            Application.Quit();
        }

        public void StopClientHost()
        {
            if (NetworkClient.active)
            {
                if (NetworkClient.isConnected)
                {
                    manager.StopHost();
                }
                manager.StopClient();
            }
            ui.SetActive(true);
        }

        private IEnumerator VerificationConnection()
        {
            yield return new WaitForSeconds(5f);
            ui.SetActive(!NetworkClient.isConnected);           
        }
    }
}
